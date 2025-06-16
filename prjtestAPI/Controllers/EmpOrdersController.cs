using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models.DTOs.LinePay;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/emporders")]
    [ApiController]
    public class EmpOrdersController : ControllerBase
    {
        private readonly IOrderService _orderSvc;
        public EmpOrdersController(IOrderService orderSvc)
            => _orderSvc = orderSvc;

        [HttpPost]
        public async Task<ActionResult<ApiResponse<int[]>>> CreateEmpOrders(
            [FromBody] CreateEmpOrderDto[] dtos)
        {
            var orderIds = await _orderSvc.CreateEmpOrdersAsync(dtos);
            return Ok(ApiResponse<int[]>.SuccessResponse(orderIds));
        }
    }
}
