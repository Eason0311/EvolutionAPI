using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using prjEvolutionAPI.Hubs;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Services.Interfaces;
using prjtestAPI.Services.Interfaces;
using System.Security.Claims;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CreateCourseController : ControllerBase
    {
        private readonly IHubContext<CourseHub> _hubContext;
        private readonly ICourseService _courseService;
        private readonly ILogger<CreateCourseController> _logger;
        private readonly IUserService _userService;
        public CreateCourseController(IUserService userService, ILogger<CreateCourseController> logger, IHubContext<CourseHub> hubContext, ICourseService courseService)
        {
            _hubContext = hubContext;
            _logger = logger;
            _courseService = courseService;
            _userService = userService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateCourse([FromForm] VCourseDTO dto)
        {


            //從JWT獲取使用者ID
            var userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == null)
                return Unauthorized(ApiResponse<string>.FailResponse("未授權的使用者"));
            var ConnectionId = dto.ConnectionId;
            var companyId = await _userService.GetUserCompanyId(userId);

            Console.WriteLine(dto);
            try
            {
                // 先 log 參數看看
                _logger.LogInformation("接收到 CompanyId: {CompanyId}", dto.CompanyId);
                _logger.LogInformation("CourseTitle: {CourseTitle}", dto.CourseTitle);
                _logger.LogInformation("IsPublic: {IsPublic}", dto.IsPublic);
                var courseId = await _courseService.CreateCourseAsync(dto, ConnectionId, companyId, _hubContext);
                return Ok(ApiResponse<int>.SuccessResponse(courseId, "課程建立成功"));
            }
            catch (ArgumentException ex)
            {
                await _hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", 100, $"❌ 建立失敗：{ex.Message}");
                return BadRequest(ApiResponse<string>.FailResponse($"參數錯誤：{ex.Message}"));
            }
            catch (Exception ex)
            {
                await _hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", 100, $"❌ 建立失敗：{ex.Message}");
                return StatusCode(500, ApiResponse<string>.FailResponse($"伺服器錯誤：{ex.Message}"));
            }
        }

        [HttpPut("{courseId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateCourse(int courseId, [FromForm] VCourseDTO dto)
        {
            var ConnectionId = dto.ConnectionId;
            if (string.IsNullOrEmpty(ConnectionId))
                return Unauthorized(ApiResponse<string>.FailResponse("未授權的使用者"));

            try
            {
                var updatedCourse = await _courseService.UpdateCourseAsync(courseId, dto, ConnectionId, _hubContext);
                return Ok(ApiResponse<bool>.SuccessResponse(updatedCourse, "課程更新成功"));
            }
            catch (ArgumentException ex)
            {
                await _hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", 100, $"❌ 更新失敗：{ex.Message}");
                return BadRequest(ApiResponse<string>.FailResponse($"參數錯誤：{ex.Message}"));
            }
            catch (Exception ex)
            {
                await _hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", 100, $"❌ 更新失敗：{ex.Message}");
                return StatusCode(500, ApiResponse<string>.FailResponse($"伺服器錯誤：{ex.Message}"));
            }
        }

        [HttpPut("final/{courseId}")]
        public async Task<IActionResult> FinalizeCourse(int courseId, VFinalDTO dto)
        {


            try
            {
                await _courseService.MarkCourseAsCompletedAsync(courseId, dto, _hubContext);
                return Ok(ApiResponse<string>.SuccessResponse("課程已完成", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse($"完成失敗：{ex.Message}"));
            }
        }

        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourse(int courseId)
        {
            try
            {
                var course = await _courseService.GetCourseAsync(courseId);
                if (course == null)
                    return NotFound(ApiResponse<string>.FailResponse("找不到課程"));
                var courseDto = new ResCourseDTO
                {
                    CourseId = course.CourseId,
                    CompanyId = course.CompanyId,
                    CourseTitle = course.CourseTitle,
                    CourseDes = course.CourseDes,
                    IsPublic = course.IsPublic,
                    CoverImage = course.CoverImagePath,
                    Price = course.Price
                };
                return Ok(ApiResponse<ResCourseDTO>.SuccessResponse(courseDto));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<string>.FailResponse($"找不到課程：{ex.Message}"));
            }
        }

        [HttpGet("learn/{courseId}")]
        public async Task<IActionResult> LearnCourse(int courseId)
        {
            try
            {
                var result = await _courseService.GetCourseLearning(courseId);
                return Ok(ApiResponse<ResFinalCourse>.SuccessResponse(result, "取得課程所有資訊成功"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.FailResponse($"取得課程所有資訊失敗：{ex.Message}"));
            }
        }
    }
}

