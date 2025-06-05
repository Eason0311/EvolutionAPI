using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Helpers;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Models.DTOs.User;
using prjEvolutionAPI.Services.Interfaces;
using prjtestAPI.Constants;
using prjtestAPI.Helpers;
using prjtestAPI.Models;
using prjtestAPI.Models.DTOs.Account;
using prjtestAPI.Services.Interfaces;
using System.Security.Claims;

namespace prjEvolutionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly EvolutionApiContext _db;
        private readonly IMailService _mailService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserActionTokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;
        private readonly ICompanyService _compenyService;
        private IUserService _userService;

        public AccountController(
            EvolutionApiContext db,
            IMailService mailService,
            IPasswordHasher passwordHasher,
            IUserActionTokenService tokenService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IJwtService jwtService,
            ICompanyService compenyService,
            IUserService userService)
        {
            _db = db;
            _mailService = mailService;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _jwtService = jwtService;
            _compenyService = compenyService;
            _userService = userService;
        }

        [HttpPost("create-company")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateCompany([FromBody] RegisterCompanyDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                    .ToList();

                var errorMsg = string.Join("；", allErrors);

                return BadRequest(
                ApiResponse<string>.FailResponse(errorMsg, null, 400)
                );
            }

            var result = await _compenyService.CreateCompanyWithAdminAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(
                    ApiResponse<string>.FailResponse(result.ErrorMessage!, null, 400));

            return Ok(ApiResponse<string>.SuccessResponse(
                data: "公司與管理員已建立，初始密碼信件已寄出",
                statusCode: 200
            ));
        }

        [HttpPost("create-employee")]
        [Authorize(Roles = "Admin")] // 限公司管理員可呼叫
        public async Task<IActionResult> CreateEmployee([FromBody] RegisterEmployeeDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<string>.FailResponse("無效的輸入", errors, 400));
            }

            // 取得登入者資訊
            int? checkUserId = User.GetUserId();
            if (checkUserId == null)
            {
                return Unauthorized(
                    ApiResponse<string>.FailResponse(
                        "使用者識別錯誤，請重新登入。",
                        null,
                        401));
            }
            int adminId = checkUserId.Value;

            var result = await _userService.CreateUserAsync(dto,adminId);

            if (!result.IsSuccess)
                return BadRequest(
                    ApiResponse<string>.FailResponse(result.ErrorMessage!, null, 400));

            return Ok(ApiResponse<string>.SuccessResponse(
                data: "員工帳號已建立，初始化密碼信件已寄出",
                statusCode: 200
            ));
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            var user = await _db.TUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Ok(ApiResponse<string>.SuccessResponse("若此信箱存在，系統將寄出重設密碼信件", 200));

            var tokenEntity = await _tokenService.CreateTokenAsync(user.UserId, UserActionTokenTypes.ResetPassword, TimeSpan.FromHours(1));
            var baseUrl = _configuration["Frontend:BaseUrl"]; // 注入 IConfiguration
            var link = $"{baseUrl}/#/reset-password?token={tokenEntity.Token}";
            var body = EmailTemplateBuilder.BuildResetPasswordEmail(user.Username, link);
            await _mailService.SendAsync(user.Email, "重設密碼通知", body);

            return Ok(ApiResponse<string>.SuccessResponse(
                data: "若此信箱存在，系統將寄出重設密碼信件",
                statusCode: 200
            ));
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var tokenEntity = await _tokenService.GetValidTokenAsync(dto.Token, UserActionTokenTypes.ResetPassword);
            if (tokenEntity == null)
                return BadRequest(ApiResponse<string>.FailResponse("Token 無效或已過期", null, 400));

            var user = tokenEntity.User;
            if (user == null)
                return NotFound(ApiResponse<string>.FailResponse("找不到使用者", null, 404));

            // 修改密碼
            user.PasswordHash = _passwordHasher.Hash(dto.NewPassword);
            user.IsEmailConfirmed = true;

            // 自動解除鎖定 & 清除失敗紀錄
            user.FailedLoginCount = 0;
            user.LockoutEndTime = null;

            // 標記 Token 已使用
            await _tokenService.MarkTokenAsUsedAsync(tokenEntity);

            // 自動產生 AccessToken 與 RefreshToken
            var accessToken = _jwtService.GenerateAccessToken(user);
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].FirstOrDefault() ?? string.Empty;
            var refreshToken = await _jwtService.GenerateRefreshToken(user.UserId, ip, userAgent);

            // 儲存變更
            _db.TRefreshTokens.Add(refreshToken);
            await _unitOfWork.CompleteAsync();

            // 回傳給前端自動登入
            return Ok(ApiResponse<object>.SuccessResponse(
            data: new
            {
                message = "密碼重設成功，已自動登入",
                accessToken,
                refreshToken = refreshToken.Token,
                username = user.Username,
                role = user.Role
            },
            statusCode: 200
            ));
        }


        [HttpPost("init-password")]
        public async Task<IActionResult> InitPassword([FromBody] InitPasswordDTO dto)
        {
            var tokenEntity = await _tokenService.GetValidTokenAsync(dto.Token, UserActionTokenTypes.InitPassword);
            if (tokenEntity == null)
                return BadRequest(ApiResponse<string>.FailResponse(
                    message: "Token 無效或已過期",
                    errors: null,
                    statusCode: 400
                ));

            var user = tokenEntity.User;
            if (user == null)
                return NotFound(ApiResponse<string>.FailResponse("找不到使用者", null, 404));

            user.PasswordHash = _passwordHasher.Hash(dto.NewPassword);
            user.IsEmailConfirmed = true;

            await _tokenService.MarkTokenAsUsedAsync(tokenEntity);
            await _unitOfWork.CompleteAsync();
            return Ok(ApiResponse<string>.SuccessResponse(
                data: "密碼設定成功，帳號已啟用",
                statusCode: 200
            ));
        }

        [HttpPost("resend-initial-password")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResendInitialPassword([FromBody] ResendInitialDTO dto)
        {
            // a. 找到該員工是否存在、且屬於同一個公司的狀況可依需求檢查
            var user = await _db.TUsers
                .FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return NotFound(ApiResponse<string>.FailResponse("找不到此員工", null, 404));

            // b. 產生一筆新的 ResetPassword Token (同樣 24 小時有效)
            var tokenEntity = await _tokenService.CreateTokenAsync(
                user.UserId,
                UserActionTokenTypes.ResetPassword,
                TimeSpan.FromHours(24)
            );

            // c. 寄信給員工
            var baseUrl = _configuration["Frontend:BaseUrl"];
            var resetLink = $"{baseUrl}/#/set-initial-password?token={tokenEntity.Token}";
            var body = EmailTemplateBuilder.BuildInitPasswordEmail(user.Username, resetLink);
            await _mailService.SendAsync(user.Email, "重新寄送：初始密碼設定通知", body);

            return Ok(ApiResponse<string>.SuccessResponse("重設連結已重新寄出", 200));
        }
    }
}
