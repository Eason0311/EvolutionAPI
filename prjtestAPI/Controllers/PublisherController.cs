using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Responses;
using prjEvolutionAPI.Services;
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

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<CompanyListDTO>>> CreateClient([FromBody] RegisterCompanyDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                    .ToList();

                var errorMsg = string.Join("；", allErrors);

                return BadRequest(
                    ApiResponse<string>.FailResponse(errorMsg, null, 400)
                );
            }

            var result = await _companyService.CreateCompanyWithAdminAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(ApiResponse<CompanyListDTO>
                    .FailResponse(result.ErrorMessage!, null, 400));

            var created = result.Data;

            var resultDto = new CompanyListDTO
            {
                CompanyId = created.CompanyId,
                CompanyName = created.CompanyName,
                CompanyEmail = created.CompanyEmail,
                IsActive = created.IsActive,
                CreatedAt = created.CreatedAt
            };

            return Ok(ApiResponse<CompanyListDTO>.SuccessResponse(
                data: resultDto,
                message: "公司與管理員已建立，初始密碼信件已寄出",
                statusCode: 200
            ));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<ApiResponse<CompanyListDTO>>> UpdateClient(int id, [FromBody] CompanyUpdateDTO dto)
        {
            if (id != dto.CompanyId)
                return BadRequest(ApiResponse<CompanyListDTO>.FailResponse("路徑參數與 Payload 不符"));

            // 1. 呼叫 Service 層更新公司
            var updated = await _companyService.UpdateCompanyAsync(dto);
            if (updated == null)
                return NotFound(ApiResponse<CompanyListDTO>.FailResponse("找不到要更新的客戶"));

            // 2. 投影成 DTO 回傳
            var resultDto = new CompanyListDTO
            {
                CompanyId = updated.CompanyId,
                CompanyName = updated.CompanyName,
                CompanyEmail = updated.CompanyEmail,
                IsActive = updated.IsActive,
                CreatedAt = updated.CreatedAt
            };
            return Ok(ApiResponse<CompanyListDTO>.SuccessResponse(resultDto));
        }
    }
}
