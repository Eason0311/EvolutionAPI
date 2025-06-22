using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Helpers;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Services.Interfaces;
using System.Security.Claims;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepListController : ControllerBase
    {
        private readonly IDepListService _depListService;
        public DepListController(IDepListService depListService)
        {
            _depListService = depListService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var userId = User.GetUserId();
            if (userId==null)
            {
                return Unauthorized(ApiResponse<string>.FailResponse("未授權的使用者"));
            }
            var userIdInt = userId.Value;
            var departments = await _depListService.GetAllDepsAsync(userIdInt);
            if (departments == null || !departments.Any())
            {
                return NotFound("No departments found.");
            }
            return Ok(ApiResponse<IEnumerable<ResDepListDTO>>.SuccessResponse(departments));
        }
    }
}
