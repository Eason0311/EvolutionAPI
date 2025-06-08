using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Home;
using prjEvolutionAPI.Services.Interfaces;
using prjtestAPI.Controllers;
using prjtestAPI.Helpers;
using prjtestAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;
        private readonly EvolutionApiContext _db;
        private readonly ICourseService _courseService;
        private readonly IHomeService _homeService;

        public HomeController
        (
            IJwtService jwtService,
            IUnitOfWork uow,
            ILogger<AuthController> logger,
            IConfiguration configuration,
            IOptions<JwtSettings> jwtOptions,
            EvolutionApiContext db,
            ICourseService courseService,
            IHomeService homeService
        )
        {
            _jwtService = jwtService;
            _uow = uow;
            _logger = logger;
            _configuration = configuration;
            _jwtSettings = jwtOptions.Value;
            _db = db;
            _courseService = courseService;
            _homeService = homeService;
        }

        [HttpGet("aboutinfo")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AboutInfoDTO>>> GetAboutInfo()
        {
            var result = await _homeService.GetAboutInfoAsync();

            return Ok(ApiResponse<AboutInfoDTO>.SuccessResponse(result));
        }
    }
}
