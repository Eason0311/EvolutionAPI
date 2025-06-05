using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using prjtestAPI.Helpers;
using prjtestAPI.Repositories.Interfaces;
using prjtestAPI.Services.Interfaces;
using prjEvolutionAPI.Helpers;
using prjtestAPI.Models;
using prjEvolutionAPI.Models.DTOs.User;
using prjEvolutionAPI.Models;

namespace prjtestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
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
        private readonly EvolutionApiContext _db;

        public UsersController
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
        IMailService mailService,
        EvolutionApiContext db
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
            _db = db;
        }

        [HttpGet("useridfo")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserInfoDTO>>> GetUserInfo()
        {
            // 1. 從 Jwt 中取出 userId
            int? checkUserId = User.GetUserId();
            if (checkUserId == null)
            {
                return Unauthorized(
                    ApiResponse<UserInfoDTO>.FailResponse(
                        "使用者識別錯誤，請重新登入。",
                        null,
                        401));
            }

            UserInfoDTO? dto = await _userService.GetUserInfoAsync(checkUserId);

            if (dto == null)
            {
                // 這裡可以區分是 404 還是 401，但既然我們已在第 1 步檢查過，這裡只要返回 404
                return NotFound(
                    ApiResponse<UserInfoDTO>.FailResponse(
                        "使用者不存在。",
                        null,
                        404));
            }

            // 4. 回傳成功結果
            return Ok(
                ApiResponse<UserInfoDTO>.SuccessResponse(
                    dto,
                    "取得成功",
                    200));
        }
    }
}