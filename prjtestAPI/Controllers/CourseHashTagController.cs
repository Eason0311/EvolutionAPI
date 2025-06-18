using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Services.Interfaces;
using System.Threading.Tasks;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseHashTagController : ControllerBase
    {   
        private readonly ICourseHashTagService _courseHashTagService;
        public CourseHashTagController(ICourseHashTagService courseHashTagService )
        {
            _courseHashTagService = courseHashTagService;
        }
        [HttpPost]
        public async Task<IActionResult> AddCourseHashTag(VCourseHashTag dto)
        {
            
            // Here you would typically call a service to handle the logic of adding the hash tag.
            // For this example, we will just return a success message.
            var res=await _courseHashTagService.CreateCourseHashTag(dto);
            return Ok(ApiResponse<bool>.SuccessResponse(res,"新增課程標籤成功"));
        }
    }
}
