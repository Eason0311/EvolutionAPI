using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class HashTagListController : ControllerBase
    {
        private readonly IHashTagListService _hashTagListService;
        public HashTagListController(IHashTagListService hashTagListService)
        {
            _hashTagListService = hashTagListService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllHashTags()
        {
            var hashTags = await _hashTagListService.GetAllHashTagsAsync();
            if (hashTags == null || !hashTags.Any())
            {
                return NotFound("No hashtags found.");
            }
            return Ok(ApiResponse<IEnumerable<ResHashTagDTO>>.SuccessResponse(hashTags));
        }
    }
}

