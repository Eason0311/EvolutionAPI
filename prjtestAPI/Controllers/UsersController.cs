using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using prjEvolutionAPI.Models.DTOs;
using prjtestAPI.Helpers;
using prjtestAPI.Repositories.Interfaces;
using prjtestAPI.Services.Interfaces;
using prjtestAPI.Data;
using prjEvolutionAPI.Helpers;

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
        private readonly TestApiContext _db;

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
        TestApiContext db
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
            int? checkUserId = User.GetUserId();

            if (checkUserId == null)
            {
                return Unauthorized(
                    ApiResponse<UserInfoDTO>.FailResponse(
                        "使用者識別錯誤，請重新登入。", null, 401));
            }

            int userId = checkUserId.Value;

            var userEntity = await _db.TUsers
                .Where(u => u.UserId == userId)
                .Select(u => new UserInfoDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();

            if (userEntity == null)
            {
                return NotFound(
                    ApiResponse<UserInfoDTO>.FailResponse(
                        "使用者不存在。", null, 404));
            }

            return Ok(
                ApiResponse<UserInfoDTO>.SuccessResponse(
                    userEntity, "取得成功", 200));

        }


    }
}