using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Services.Interfaces;

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
            var departments = await _depListService.GetAllDepsAsync();
            if (departments == null || !departments.Any())
            {
                return NotFound("No departments found.");
            }
            return Ok(ApiResponse<IEnumerable<ResDepListDTO>>.SuccessResponse(departments));
        }
    }
}
