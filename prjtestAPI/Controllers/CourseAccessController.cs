using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Services.Interfaces;

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
            if (dto == null || dto.courseId <= 0)
            {
                return BadRequest("Invalid data provided.");
            }
            var result = await _courseAccessService.CreateCourseAccessAsync(dto);
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
