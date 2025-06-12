using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models.DTOs.LinePay;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinePayController : ControllerBase
    {
        private readonly ILinePayService _svc;
        public LinePayController(ILinePayService svc) => _svc = svc;

        // 1) 預約付款
        [HttpPost("request")]
        public async Task<IActionResult> Request([FromBody] PaymentRequestDto dto)
        {
            var result = await _svc.RequestPaymentAsync(dto.Amount, dto.OrderId, dto.ProductName);
            return Ok(new
            {
                transactionId = result.Info.TransactionId,
                paymentUrl = result.Info.PaymentUrl.Web
            });
        }

        // 2) 確認付款
        [HttpGet("confirm")]
        public async Task<IActionResult> Confirm([FromQuery] string transactionId, [FromQuery] decimal amount)
        {
            // -- 這裡要：
            //  a. 從 DB 驗證 orderId 與 amount 是否一致
            //  b. 呼叫 _svc.ConfirmPaymentAsync(transactionId, amount)
            //  c. 更新訂單狀態為「已付款」
            //  d. 回傳給前端或做 Redirect
            var confirmResult = await _svc.ConfirmPaymentAsync(transactionId, amount);
            // TODO: 更新資料庫、做分流
            return Ok(confirmResult);
        }
    }
}
