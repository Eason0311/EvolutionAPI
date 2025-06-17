using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Helpers;
using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Models.DTOs.CompanyManage;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Responses;
using prjEvolutionAPI.Services.Interfaces;
using prjtestAPI.Models.DTOs.Account;
using prjtestAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyManageController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IUserService _userService;
        public CompanyManageController(ICompanyService companyService,IUserService userService)
        {
            _companyService = companyService;
            _userService = userService;
        }
        [HttpGet("employeesList")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        //[AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PagedResult<EmployeesListDTO>>>> GetPagedResult(
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

            int? checkUserId = User.GetUserId();
            if (checkUserId == null)
            {
                return Unauthorized(
                    ApiResponse<PagedResult<EmployeesListDTO>>.FailResponse(
                        "使用者識別錯誤，請重新登入。",
                        null,
                        401));
            }
            int adminId = checkUserId.Value;

            var comId = await _userService.GetUserCompanyIdAsync(adminId);

            // 2. 呼叫 Service 拿分頁結果
            var paged = await _userService.GetClientsPagedAsync(
                start, limit, sortField, sortOrder, filters, comId);

            return Ok(ApiResponse<PagedResult<EmployeesListDTO>>.SuccessResponse(paged));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<ApiResponse<EmployeesListDTO>>> CreateEmployee([FromBody] RegisterEmployeeDTO dto)
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

            int? checkUserId = User.GetUserId();
            if (checkUserId == null)
            {
                return Unauthorized(
                    ApiResponse<string>.FailResponse(
                        "使用者識別錯誤，請重新登入。",
                        null,
                        401));
            }
            int adminId = checkUserId.Value;

            var result = await _userService.CreateUserAsync(dto, adminId);

            if (!result.IsSuccess)
                return BadRequest(ApiResponse<EmployeesListDTO>
                    .FailResponse(result.ErrorMessage!, null, 400));

            var created = result.Data;

            var resultDto = new EmployeesListDTO
            {
                UserId = created.UserId,
                Username = created.Username,
                Email = created.Email,
                UserDep = created.UserDepNavigation.DepName,
                UserStatus = created.UserStatus
            };

            return Ok(ApiResponse<EmployeesListDTO>.SuccessResponse(
                data: resultDto,
                message: "員工帳號已建立，初始化密碼信件已寄出",
                statusCode: 200
            ));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<ApiResponse<EmployeesListDTO>>> UpdateEmployee(int id, [FromBody] EmployeeUpdateDTO dto)
        {
            if (id != dto.UserId)
                return BadRequest(ApiResponse<EmployeesListDTO>.FailResponse("路徑參數與 Payload 不符"));

            // 1. 呼叫 Service 層更新公司
            var updated = await _userService.UpdateEmployeeAsync(dto);
            if (updated == null)
                return NotFound(ApiResponse<EmployeesListDTO>.FailResponse("找不到要更新的客戶"));

            // 2. 投影成 DTO 回傳
            var resultDto = new EmployeesListDTO
            {
                UserId = updated.UserId,
                Username = updated.Username,
                Email = updated.Email,
                UserStatus = updated.UserStatus,
                UserDep = updated.UserDepNavigation.DepName,
            };
            return Ok(ApiResponse<EmployeesListDTO>.SuccessResponse(resultDto));
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<ApiResponse<EmployeesListDTO>>> UpdateStatus(
    int id,
    [FromBody] EmployeeStatusUpdateDTO dto)
        {
            if (id != dto.UserId)
                return BadRequest(ApiResponse<EmployeesListDTO>.FailResponse("路徑參數與 Payload 不符"));

            // 呼叫 Service 只把狀態改成 Inactive
            var updated = await _userService.UpdateStatusAsync(id);
            if (updated == null)
                return NotFound(ApiResponse<EmployeesListDTO>.FailResponse("找不到使用者"));

            var resultDto = new EmployeesListDTO
            {
                UserId = updated.UserId,
                Username = updated.Username,
                Email = updated.Email,
                UserDep = updated.UserDepNavigation.DepName,
                UserStatus = updated.UserStatus
            };
            return Ok(ApiResponse<EmployeesListDTO>.SuccessResponse(resultDto));
        }

        [HttpPost("batch/status")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<ApiResponse<int>>> UpdateStatusesBulk(
    [FromBody] int[] userIds)
        {
            if (userIds == null || userIds.Length == 0)
                return BadRequest(ApiResponse<int>.FailResponse("未提供任何使用者 ID"));

            // 呼叫 Service 回傳實際被停用的人數
            var count = await _userService.UpdateStatusesBulkAsync(userIds);

            return Ok(ApiResponse<int>.SuccessResponse(count));
        }
    }
}