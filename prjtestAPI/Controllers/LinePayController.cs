using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Models.DTOs.LinePay;
using prjEvolutionAPI.Responses;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [ApiController]
    [Route("api/linepay")]
    public class LinePayController : ControllerBase
    {
        private readonly ILinePayService _linePay;
        private readonly IPaymentService _paymentSvc;
        private readonly IOrderService _orderSvc;
        private readonly LinePayOptions _opt;

        public LinePayController(
            IOptions<LinePayOptions> opts,
            ILinePayService linePay,
            IPaymentService paymentSvc,
            IOrderService orderSvc)
        {
            _opt = opts.Value;
            _linePay = linePay;
            _paymentSvc = paymentSvc;
            _orderSvc = orderSvc;
        }

        [HttpPost("request")]
        public async Task<ActionResult<ApiResponse<LinePayRequestInfo>>> Request(
       [FromBody] PaymentRequestDto dto)
        {
            // 1. 先在 DB 建 Pending 紀錄
            var paymentId = await _paymentSvc.CreatePaymentAsync(
                dto.Amount, "Pending", null,
                new[]{ (dto.OrderId.StartsWith("C") ? (int?)int.Parse(dto.OrderId.Substring(1)) : null,
                    dto.OrderId.StartsWith("E") ? (int?)int.Parse(dto.OrderId.Substring(1)) : null) }
            );

            // 2. 呼叫 LINE Pay
            var resp = await _linePay.RequestPaymentAsync(
                dto.Amount,
                dto.OrderId,
                dto.ProductName,
                _opt.ConfirmUrl,
                _opt.CancelUrl
            );
            // 3. 更新 TransactionId 並寫入明細
            await _paymentSvc.UpdateTransactionAsync(paymentId, resp.Info!.TransactionId);

            var info = new LinePayRequestInfo
            {
                TransactionId = resp.Info.TransactionId,
                PaymentUrl = resp.Info.PaymentUrl
            };
            return Ok(ApiResponse<LinePayRequestInfo>.SuccessResponse(info));
        }

        [AllowAnonymous]
        [HttpGet("confirm")]
        public async Task<IActionResult> Confirm(
    [FromQuery] string transactionId,
    [FromQuery] bool isCompany)    // 如果你還需要 isCompany，就保留
        {
            // 1. 先用 transactionId 撈付款紀錄
            if (!long.TryParse(transactionId, out var txId))
                return BadRequest(ApiResponse<object>.FailResponse("transactionId 格式錯誤"));

            var payment = await _paymentSvc.GetByTransactionIdAsync(txId);
            if (payment == null)
                return BadRequest(ApiResponse<object>.FailResponse("找不到付款紀錄"));

            // 2. 呼叫 LINE Pay Confirm
            var confirm = await _linePay.ConfirmPaymentAsync(transactionId, payment.Amount);

            // 3. 更新狀態
            var status = confirm.ReturnCode == "0000" ? "Paid" : "Failed";
            await _paymentSvc.UpdateStatusAsync(payment.PaymentId, status);

            // 4. 標記訂單已付款
            await _orderSvc.MarkOrderPaidAsync(payment.PaymentId, isCompany);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                transactionId = confirm.Info.TransactionId,
                amount = payment.Amount
            }));
        }
    }
}