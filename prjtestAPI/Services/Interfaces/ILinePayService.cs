using prjEvolutionAPI.Responses;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface ILinePayService
    {
        /// 向 LINE Pay 發出「付款預約」請求，回傳 transactionId 與 paymentUrl。
        Task<LinePayRequestResponse> RequestPaymentAsync(
            decimal amount,
            string orderId,
            string productName);

        /// 使用 transactionId 與金額向 LINE Pay 呼叫「付款確認」API。
        Task<LinePayConfirmResponse> ConfirmPaymentAsync(
            string transactionId,
            decimal amount);
    }
}
