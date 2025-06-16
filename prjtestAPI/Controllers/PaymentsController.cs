using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Responses;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _svc;
        public PaymentsController(IPaymentService svc) => _svc = svc;

        public class CreatePaymentDto
        {
            public decimal Amount { get; set; }
            public string Status { get; set; } = "Pending";
            public long? TransactionId { get; set; }
            public List<DetailDto> Details { get; set; } = new();
        }
        public class DetailDto
        {
            public int? CompOrderId { get; set; }
            public int? EmpOrderId { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<object>>> Create([FromBody] CreatePaymentDto dto)
        {
            try
            {
            var tuples = dto.Details
                .Select(x => (x.CompOrderId, x.EmpOrderId));

            var paymentId = await _svc.CreatePaymentAsync(
                dto.Amount, dto.Status, dto.TransactionId, tuples);

            var payload = new { PaymentId = paymentId };
            return CreatedAtAction(
                nameof(GetByTransactionId),
                new { transactionId = paymentId },
                ApiResponse<object>.SuccessResponse(payload)
            );
            }
            catch (Exception ex)
            {
                // 1) 把完整例外寫到日誌
                // _logger.LogError(ex, "Create payment failed");
                // 2) 回一個乾淨的 JSON 錯誤
                return StatusCode(500,
                  ApiResponse<string>.FailResponse($"伺服器錯誤：{ex.Message}", null, 500));
            }
        }

        [HttpGet("{transactionId}")]
        public async Task<ActionResult<ApiResponse<TPayment>>> GetByTransactionId(long transactionId)
        {
            var payment = await _svc.GetByTransactionIdAsync(transactionId);
            if (payment == null)
                return NotFound(ApiResponse<TPayment>.FailResponse("找不到對應的付款紀錄"));

            return Ok(ApiResponse<TPayment>.SuccessResponse(payment));
        }
    }
}
