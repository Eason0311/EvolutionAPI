using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Services.Interfaces;
using System.Security.Claims;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseAccessController : ControllerBase
    {
        private readonly ICourseAccessService _courseAccessService;
        public CourseAccessController(ICourseAccessService courseAccessService)
        {
            _courseAccessService = courseAccessService;
        }
        [HttpPost]
        public async Task<IActionResult> AddCourseAccess(VCourseAccessDTO dto)
        {
            //從JWT獲取使用者ID
            var userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (dto == null || dto.courseId <= 0)
            {
                return BadRequest("Invalid data provided.");
            }
            var result = await _courseAccessService.CreateCourseAccessAsync(dto, userId);
            if (result)
            {
                return Ok(ApiResponse<bool>.SuccessResponse(result, "Course access added successfully."));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding course access.");
            }
        }
    }
}
