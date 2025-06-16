using System.ComponentModel.Design;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using prjEvolutionAPI.Helpers;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Models.DTOs.CompanyManage;
using prjEvolutionAPI.Models.DTOs.Course;
using prjEvolutionAPI.Models.DTOs.LinePay;
using prjEvolutionAPI.Responses;
using prjEvolutionAPI.Services.Interfaces;
using prjtestAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [ApiController]
    [Route("api/linepay")]
    public class LinePayController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILinePayService _linePay;
        private readonly IPaymentService _paymentSvc;
        private readonly IOrderService _orderSvc;
        private readonly LinePayOptions _opt;

        public LinePayController(
            IUserService userService,
            IOptions<LinePayOptions> opts,
            ILinePayService linePay,
            IPaymentService paymentSvc,
            IOrderService orderSvc)
        {
            _opt = opts.Value;
            _linePay = linePay;
            _paymentSvc = paymentSvc;
            _orderSvc = orderSvc;
            _userService = userService;
        }

        [HttpPost("request")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<LinePayRequestInfo>>> RequestPaymentAsync(
    [FromBody] List<CourseDTO> cartItems)
        {
            int? checkUserId = User.GetUserId();
            if (checkUserId == null)
            {
                return Unauthorized(ApiResponse<LinePayRequestInfo>.FailResponse("使用者識別錯誤，請重新登入。", null, 401));
            }

            int userId = checkUserId.Value;
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            int companyId = await _userService.GetUserCompanyIdAsync(userId);

            int totalAmount = 0;
            var orderDetails = new List<(int? compOrderId, int? empOrderId)>();

            foreach (var item in cartItems)
            {
                totalAmount += item.Price;

                if (isAdmin)
                {
                    var order = new TCompOrder
                    {
                        BuyerCompanyId = companyId,
                        CourseId = item.CourseId,
                        OrderDate = DateTime.UtcNow,
                        Amount = item.Price,
                        IsPaid = false
                    };
                    int orderId = await _orderSvc.CreateCompOrderAsync(order);
                    orderDetails.Add((compOrderId: orderId, empOrderId: null));
                }
                else
                {
                    var order = new TEmpOrder
                    {
                        BuyerUserId = userId,
                        CourseId = item.CourseId,
                        OrderDate = DateTime.UtcNow,
                        Amount = item.Price,
                        IsPaid = false
                    };
                    int orderId = await _orderSvc.CreateEmpOrderAsync(order);
                    orderDetails.Add((compOrderId: null, empOrderId: orderId));
                }
            }

            int paymentId = await _paymentSvc.CreatePaymentAsync(
                totalAmount,
                status: "Pending",
                transactionId: null,
                details: orderDetails
            );

            string orderIdForLinePay = $"EV-{paymentId:D8}";

            // ✅ 使用你從 user secret 取得的 _opt（已經在建構子中 .Value 過了）
            string confirmUrl = $"{_opt.ConfirmUrl}?orderId={orderIdForLinePay}";
            string cancelUrl = _opt.CancelUrl;

            var linePayResponse = await _linePay.RequestPaymentAsync(
                amount: totalAmount,
                orderId: orderIdForLinePay,
                productName: "線上課程購買",
                confirmUrl: confirmUrl,
                cancelUrl: cancelUrl
            );
            await _paymentSvc.UpdateTransactionAsync(paymentId, linePayResponse.Info.TransactionId);

            return Ok(ApiResponse<LinePayRequestInfo>.SuccessResponse(new LinePayRequestInfo
            {
                PaymentId = paymentId,
                TransactionId = linePayResponse.Info.TransactionId,
                PaymentUrl = linePayResponse.Info.PaymentUrl
            }));
        }

        [AllowAnonymous]
        [HttpGet("confirm")]
        public async Task<IActionResult> Confirm(
            [FromQuery] string transactionId,
            [FromQuery] bool isCompany)
        {
            // 1. 轉型 transactionId
            if (!long.TryParse(transactionId, out long tid))
                return BadRequest("交易編號格式錯誤");

            // 2. 根據 transactionId 取付款資料
            var payment = await _paymentSvc.GetByTransactionIdAsync(tid);
            if (payment == null)
                return NotFound("找不到此交易紀錄");

            // 3. 呼叫 LINE Pay 確認 API
            var result = await _linePay.ConfirmPaymentAsync(
            transactionId: transactionId,
             amount: payment.Amount
            );

            // 4. 若付款成功
            if (result.ReturnCode == "0000")
            {
                await _paymentSvc.MarkPaymentAsPaidAsync(tid);
                await _orderSvc.MarkOrdersPaidByPaymentIdAsync(payment.PaymentId);

                var frontendSuccessUrl = $"http://localhost:4200/#/payment/success?orderId={payment.PaymentId}";
                return Redirect(frontendSuccessUrl);
            }
            else
            {
                var frontendFailUrl = $"http://localhost:4200/#/payment/fail?orderId={payment.PaymentId}";
                return Redirect(frontendFailUrl);
            }
        }
    }
}