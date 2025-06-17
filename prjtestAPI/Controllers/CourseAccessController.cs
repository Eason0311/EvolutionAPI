using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.CourseBgList;
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
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourseAccess(int courseId)
        {
            if (courseId <= 0)
            {
                return BadRequest("Invalid course ID provided.");
            }

            var result = await _courseAccessService.GetCourseAccessAsync(courseId);
            if (result != null)
            {
                return Ok(ApiResponse<IEnumerable<TCourseAccess[]>>.SuccessResponse(result, "Course access retrieved successfully."));
            }
            else
            {
                return NotFound("No access found for the specified course.");
            }
        }
        [HttpPost("addUserAccess")]
        public async Task<IActionResult> AddCourseAccess(VAddCourseAccessDTO dto)
        {

            var result = await _courseAccessService.AddCourseAccessAsync(dto);
            if (result != null)
            {
                return Ok(ApiResponse<int>.SuccessResponse(result, "CourseAccess added successfully."));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the course.");
            }
        }
        [HttpDelete("{courseAccessId}")]
        public async Task<IActionResult> DeleteCourseAccess(int courseAccessId)
        {
            if (courseAccessId <= 0)
            {
                return BadRequest("Invalid course access ID provided.");
            }

            var result = await _courseAccessService.DelUserAccess(courseAccessId);
            if (result)
            {
                return Ok(ApiResponse<bool>.SuccessResponse(result, "Course access deleted successfully."));
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting course access.");
            }
        }
        
    }
}
