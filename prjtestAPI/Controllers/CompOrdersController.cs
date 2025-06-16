using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models.DTOs.LinePay;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/comporders")]
    [ApiController]
    public class CompOrdersController : ControllerBase
    {
        private readonly IOrderService _orderSvc;
        public CompOrdersController(IOrderService orderSvc)
            => _orderSvc = orderSvc;

        // 2. 接收前端傳過來的 DTO 陣列
        [HttpPost]
        public async Task<ActionResult<ApiResponse<int[]>>> CreateOrders(
            [FromBody] CreateCompOrderDto[] dtos)
        {
            // 3. 呼叫 Service 建立訂單，回傳 ID 陣列
            var orderIds = await _orderSvc.CreateCompOrdersAsync(dtos);

            // 4. 回 200 + 包 data
            return Ok(ApiResponse<int[]>.SuccessResponse(orderIds));
        }
    }
}
