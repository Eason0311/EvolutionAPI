using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using prjtestAPI.Attributes;
using prjtestAPI.Constants;
using prjtestAPI.Helpers;
using prjtestAPI.Models;
using prjtestAPI.Repositories.Interfaces;
using prjtestAPI.Services;
using prjtestAPI.Services.Interfaces;

namespace prjtestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserActionTokenService _tokenService;
        private readonly IMailService _mailService;

        public AuthController
        (
        IJwtService jwtService,
        IUnitOfWork unitOfWork,
        IUserRepository userRepo,
        IRefreshTokenRepository refreshRepo,
        ILogger<AuthController> logger,
        IConfiguration configuration,
        IUserService userService,
        IOptions<JwtSettings> jwtOptions,
        IPasswordHasher passwordHasher,
        IUserActionTokenService tokenService,
        IMailService mailService
        )
        {
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
            _refreshRepo = refreshRepo;
            _logger = logger;
            _configuration = configuration;
            _userService = userService;
            _jwtSettings = jwtOptions.Value;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _mailService = mailService;
        }

        // 註冊
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode(400, ApiResponse<string>.FailResponse("無效的輸入", errors, 400));
            }

            var existingUser = await _userRepo.GetByEmailAsync(model.Email);
            if (existingUser != null)
                return StatusCode(400, ApiResponse<string>.FailResponse("該電子郵件已被註冊", null, 400));

            var user = new TUser
            {
                Email = model.Email,
                Username = model.Username,
                PasswordHash = _passwordHasher.Hash(model.Password),
                Role = "User",
                UserStatus = "Active"
            };

            await _userRepo.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return Ok(ApiResponse<string>.SuccessResponse("註冊成功", 200));
        }


        // 登入
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
                var result = await _userService.ValidateUserAsync(model.Email, model.Password);

                if (!result.EmailExists)
                    return StatusCode(404, ApiResponse<string>.FailResponse("找不到此電子郵件", null, 404));

                if (result.User == null)
                    return StatusCode(500, ApiResponse<string>.FailResponse("使用者資料載入失敗", null, 500));

                var user = result.User;


                // 檢查帳號是否鎖定
                if (user.LockoutEndTime.HasValue && user.LockoutEndTime > DateTime.UtcNow)
                {
                    var minutesLeft = (int)(user.LockoutEndTime.Value - DateTime.UtcNow).TotalMinutes;
                    return StatusCode(423, ApiResponse<string>.FailResponse($"帳號已鎖定，請於 {minutesLeft} 分鐘後再試", null, 423));
                }

                // 密碼錯誤
                if (!result.PasswordValid)
                {
                    user.FailedLoginCount = (user.FailedLoginCount ?? 0) + 1;

                    if (user.FailedLoginCount >= 3)
                    {
                        user.LockoutEndTime = DateTime.UtcNow.AddMinutes(5);

                        if (user.FailedLoginCount == 3) // 第三次當下才寄信
                        {
                            var tokenEntity = await _tokenService.CreateTokenAsync(
                                user.UserId,
                                UserActionTokenTypes.ResetPassword,
                                TimeSpan.FromHours(1)
                            );

                            var frontendBaseUrl = _configuration["Frontend:BaseUrl"];
                            var resetLink = $"{frontendBaseUrl}/#/reset-password?token={tokenEntity.Token}";
                            var emailBody = EmailTemplateBuilder.BuildResetPasswordEmail(user.Username ?? user.Email, resetLink);

                            await _mailService.SendAsync(user.Email, "帳號鎖定通知", emailBody);
                        }
                    }

                    await _unitOfWork.CompleteAsync();

                    var msg = user.FailedLoginCount >= 3
                        ? "密碼錯誤，帳號已鎖定 5 分鐘，請至信箱收取重設密碼信件"
                        : $"密碼錯誤，已失敗 {user.FailedLoginCount} 次";

                    return StatusCode(401, ApiResponse<string>.FailResponse(msg, null, 401));
                }

                // 密碼正確 → 重置鎖定狀態
                user.FailedLoginCount = 0;
                user.LockoutEndTime = null;

                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshTokenEntity = await _jwtService.GenerateRefreshToken(
                    user.UserId,
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    HttpContext.Request.Headers["User-Agent"].ToString());

                await _refreshRepo.AddAsync(refreshTokenEntity);
                await _unitOfWork.CompleteAsync();

                var response = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshTokenEntity.Token,
                    ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60
                };

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "登入成功", 200));

            }
            catch (Exception ex)
            {
                // ✅ 記錄 Log（可選）
                _logger.LogError(ex, "Login 發生未預期錯誤");

                return StatusCode(500, ApiResponse<string>.FailResponse("伺服器發生錯誤，請稍後再試", null, 500));
            }
        }

        // 更新 Token
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            // 1. 查詢 Refresh Token 紀錄
            var tokenEntity = await _refreshRepo.GetByTokenAsync(dto.RefreshToken);

            // 2. 驗證 Refresh Token 是否存在、是否撤銷、是否過期
            if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiryDate < DateTime.UtcNow)
                return StatusCode(401, ApiResponse<string>.FailResponse("無效、過期或已撤銷的 Refresh Token", null, 401));

            // 3. 查詢對應的使用者
            var user = await _userRepo.GetByIdAsync(tokenEntity.UserId);
            if (user == null)
                return StatusCode(401, ApiResponse<string>.FailResponse("使用者不存在", null, 401));


            // 4. 標示舊的 Refresh Token 為已撤銷（防止重複使用）
            tokenEntity.IsRevoked = true;
            _refreshRepo.Update(tokenEntity); // 確保你有實作 Update 方法
            await _unitOfWork.CompleteAsync();

            // 5. 發行新的 Access Token 與 Refresh Token
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshTokenEntity = await _jwtService.GenerateRefreshToken(
                user.UserId,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.Request.Headers["User-Agent"].ToString());

            await _refreshRepo.AddAsync(newRefreshTokenEntity);
            await _unitOfWork.CompleteAsync();

            // 6. 回傳新的 Token 給前端
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenEntity.Token,
                ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60
            }, "Token 更新成功", 200));
        }


        // 登出
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
        {
            if (string.IsNullOrEmpty(dto.RefreshToken))
                return StatusCode(400, ApiResponse<string>.FailResponse("Refresh Token 不可為空", null, 400));

            var tokenEntity = await _refreshRepo.GetByTokenAsync(dto.RefreshToken);
            if (tokenEntity == null)
                return StatusCode(400, ApiResponse<string>.FailResponse("無效的 Refresh Token", null, 400));


            tokenEntity.IsRevoked = true;
            _refreshRepo.Update(tokenEntity);
            await _unitOfWork.CompleteAsync();

            // 前端需自行清除 AccessToken 和 RefreshToken (如存在 localStorage / cookie)

            return Ok(ApiResponse<string>.SuccessResponse("登出成功", 200));
        }

        // 測試保護端點 (示範角色限制)
        [HttpGet("protected")]
        [RequiredRole("Admin", "SuperAdmin")]
        public IActionResult ProtectedEndpoint()
        {
            return Ok(ApiResponse<string>.SuccessResponse("存取成功，您擁有 Admin 以上的權限 !", 200));
        }

        // 顯示登入者資訊
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return Unauthorized(ApiResponse<string>.FailResponse("尚未登入", null, 401));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var name = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(ApiResponse<object>.SuccessResponse(new { userId, name, role }, "使用者資訊", 200));
        }
    }

    // DTOs
    public class RegisterDto
    {
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; } = null!;
    }

    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public int ExpiresIn { get; set; } // 單位: 秒
    }
}
