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
using prjtestAPI.Helpers;
using prjtestAPI.Services;
using prjtestAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [ApiController]
    [Route("api/linepay")]
    public class LinePayController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IUserService _userService;
        private readonly ILinePayService _linePay;
        private readonly IPaymentService _paymentSvc;
        private readonly IOrderService _orderSvc;
        private readonly LinePayOptions _opt;
        private readonly IMailService _mailService;

        public LinePayController(
            ICompanyService companyService,
            IMailService mailService,
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
            _mailService = mailService;
            _companyService = companyService;
        }

        [HttpPost("request")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<LinePayRequestInfo>>> RequestPaymentAsync(
    [FromBody] List<CourseDTO> cartItems)
        {
            try
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
                            IsPaid = false,
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
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<LinePayRequestInfo>.FailResponse("伺服器內部錯誤，請稍後再試。", null, 500));
            }
        }

        [AllowAnonymous]
        [HttpGet("confirm")]
        public async Task<IActionResult> Confirm(
        [FromQuery] string transactionId,
        [FromQuery] string orderId)
        {
            // 1. 驗證 transactionId
            if (!long.TryParse(transactionId, out long tid))
                return Redirect($"http://localhost:4200/#/payment/fail?orderId={orderId}");

            // 2. 驗證 orderId 格式 (EV-XXXXXXXX)
            if (!orderId.StartsWith("EV-") ||
                !int.TryParse(orderId["EV-".Length..], out int paymentId))
            {
                return Redirect($"http://localhost:4200/#/payment/fail?orderId={orderId}");
            }

            // 3. 取得 TPayment
            var payment = await _paymentSvc.GetByIdAsync(paymentId);
            if (payment == null)
                return Redirect($"http://localhost:4200/#/payment/fail?orderId={orderId}");

            // 4. 呼叫 LINE Pay 確認付款
            decimal paidAmount = payment.Amount;
            var linePayResult = await _linePay.ConfirmPaymentAsync(transactionId, paidAmount);
            if (linePayResult.ReturnCode != "0000")
                return Redirect($"http://localhost:4200/#/payment/fail?orderId={orderId}");

            // 5. 更新付款狀態
            await _paymentSvc.MarkPaymentAsPaidAsync(tid);

            // 6. 撈出所有 PaymentDetail，判斷公司 / 員工
            var details = await _paymentSvc.GetPaymentDetailsAsync(paymentId);
            var first = details.FirstOrDefault()
                          ?? throw new InvalidOperationException("此付款沒有任何訂單明細");

            bool isCompany = first.CompOrderId.HasValue;

            // 7. 取得收件人信箱與顯示名稱
            string userEmail, displayName;
            if (isCompany)
            {
                var compOrder = await _orderSvc.GetCompOrderByIdAsync(first.CompOrderId!.Value);
                if (compOrder == null)
                    return Redirect($"http://localhost:4200/#/payment/fail?orderId={orderId}");

                var comp = await _companyService.GetByIdAsync(compOrder.BuyerCompanyId);
                if (comp == null)
                    return Redirect($"http://localhost:4200/#/payment/fail?orderId={orderId}");

                userEmail = comp.CompanyEmail;
                displayName = compOrder.BuyerCompany.CompanyName;
            }
            else
            {
                var empOrder = await _orderSvc.GetEmpOrderByIdAsync(first.EmpOrderId!.Value);
                if (empOrder == null)
                    return Redirect($"http://localhost:4200/#/payment/fail?orderId={orderId}");

                var user = await _userService.GetByIdAsync(empOrder.BuyerUserId);
                if (user == null)
                    return Redirect($"http://localhost:4200/#/payment/fail?orderId={orderId}");

                userEmail = user.Email;
                displayName = user.Username;
            }

            // 8. 產生郵件內容
            var itemDetails = await _orderSvc.GetOrderDetailsAsync(paymentId);
            var htmlBody = EmailTemplateBuilder.BuildOrderDetailEmail(
                displayName,
                transactionId,
                paymentId,
                itemDetails,
                paidAmount,
                payment.PaidAt ?? DateTime.Now);

            // 9. 寄出消費明細郵件
            await _mailService.SendAsync(
                userEmail,
                $"您的訂單明細 (#{paymentId})",
                htmlBody);

            // 10. 更新所有下屬訂單為已付款
            await _orderSvc.MarkOrdersPaidByPaymentIdAsync(paymentId);

            // 11. 導回前端成功頁
            return Redirect($"http://localhost:4200/#/payment/success?orderId={payment.PaymentId}");
        }
    }
}