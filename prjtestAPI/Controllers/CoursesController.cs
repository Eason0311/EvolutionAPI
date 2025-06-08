using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Course;
using prjEvolutionAPI.Services.Interfaces;
using prjtestAPI;
using prjtestAPI.Controllers;
using prjtestAPI.Helpers;
using prjtestAPI.Repositories.Interfaces;
using prjtestAPI.Services;
using prjtestAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;
        private readonly EvolutionApiContext _db;
        private readonly ICourseService _courseService;

        public CoursesController
        (
        IJwtService jwtService,
        IUnitOfWork uow,
        ILogger<AuthController> logger,
        IConfiguration configuration,
        IOptions<JwtSettings> jwtOptions,
        EvolutionApiContext db,
        ICourseService courseService
        )
        {
            _jwtService = jwtService;
            _uow = uow;
            _logger = logger;
            _configuration = configuration;
            _jwtSettings = jwtOptions.Value;
            _db = db;
            _courseService = courseService;
        }

        [HttpGet("paged")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PagedResult<CourseDTO>>>> GetPaged(
            [FromQuery] int pageIndex = 0,
            [FromQuery] int pageSize = 20)
        {
            // 1. 呼叫 Service，取得該頁資料 + 總筆數
            var paged = await _courseService.GetPagedAsync(pageIndex, pageSize);

            // 2. 組成標準回應
            return Ok(ApiResponse<PagedResult<CourseDTO>>.SuccessResponse(paged));
        }

        [HttpGet("course/{courseid:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<CourseDTO>>> GetCourse(int courseid)
        {
            var result = await _uow.Course.GetByIdAsync(courseid);
            if (result == null)
                return NotFound();

            return Ok(ApiResponse<CourseDTO>.SuccessResponse(result));
        }

        [HttpPost("batch")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<IEnumerable<CourseDTO>>>> GetCoursesByIds(
       [FromBody] IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                return Ok(ApiResponse<IEnumerable<CourseDTO>>.SuccessResponse(
                    Enumerable.Empty<CourseDTO>()));

            var dtos = await _courseService.GetCoursesByIdsAsync(ids);
            return Ok(ApiResponse<IEnumerable<CourseDTO>>.SuccessResponse(dtos));
        }

        [HttpGet("suggestions")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<string>>>> Suggestions([FromQuery] string prefix)
        {
            var data = await _uow.Course.GetTitleSuggestionsAsync(prefix);
            return Ok(ApiResponse<List<string>>.SuccessResponse(data));
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<CourseDTO>>>> Search([FromQuery] string query)
        {
            var data = await _uow.Course.SearchAsync(query);
            return Ok(ApiResponse<List<CourseDTO>>.SuccessResponse(data));
        }
    }
}
