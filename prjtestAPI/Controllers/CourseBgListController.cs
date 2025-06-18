using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models.DTOs.CourseBgList;
using prjEvolutionAPI.Services.Interfaces;
using System.Security.Claims;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseBgListController : ControllerBase
    {
        private readonly ICourseBgListService _courseBgListService;
        public CourseBgListController(ICourseBgListService courseBgListService)
        {
            _courseBgListService = courseBgListService;
        }
        [HttpGet]
        public async Task<IActionResult> GetCoursePageAsync()
        {
            var service = HttpContext.RequestServices.GetService<prjEvolutionAPI.Services.Interfaces.ICourseBgListService>();
            if (service == null)
            {
                return NotFound("Service not found");
            }
            //從JWT獲取使用者ID
            var userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var courses = await service.GetCoursePageAsync(userId);
            return Ok(ApiResponse<IEnumerable<ResCourseBgListDTO>>.SuccessResponse(courses,"取得所有課程成功"));
        }
        [HttpGet("employee")]
        public async Task<IActionResult> GetEmployeeByCourseIdAsync()
        {
            var userId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var userList = await _courseBgListService.GetEmployeeAsync(userId);

            return Ok(ApiResponse<IEnumerable<ResUserBgListDTO>>.SuccessResponse(userList, "取得課程成功"));
        }
    }
}
