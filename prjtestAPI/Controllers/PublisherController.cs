using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Responses;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly IPublisherService _publisher;
        private readonly ICompanyService _companyService;
        public PublisherController(IPublisherService publisher, ICompanyService companyService)
        {
            _publisher = publisher;
            _companyService = companyService;
        }

        [HttpGet("client")]
        [Authorize(Roles = "SuperAdmin")]
        //[AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PagedResult<CompanyListDTO>>>> GetPagedResult(
        [FromQuery] int start,
        [FromQuery] int limit,
        [FromQuery] string sortField,
        [FromQuery] int sortOrder)
        {
            // 1. 取所有 QueryString 以 filter_ 開頭的參數
            var filters = HttpContext.Request.Query
                .Where(q => q.Key.StartsWith("filter_"))
                .ToDictionary(
                   q => q.Key.Substring("filter_".Length),    // e.g. companyName
                   q => q.Value.ToString()                    // e.g. "Foo"
                );

            // 2. 呼叫 Service 拿分頁結果
            var paged = await _companyService.GetClientsPagedAsync(
                start, limit, sortField, sortOrder, filters);

            return Ok(ApiResponse<PagedResult<CompanyListDTO>>.SuccessResponse(paged));
        }
    }
}
