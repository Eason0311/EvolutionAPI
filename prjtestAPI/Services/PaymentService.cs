using prjEvolutionAPI.Models;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _uow;
        public PaymentService(IUnitOfWork uow) => _uow = uow;

        public async Task<int> CreatePaymentAsync(
            decimal amount,
            string status,
            long? transactionId,
            IEnumerable<(int? compOrderId, int? empOrderId)> details)
        {
            // 1. 建立主檔
            var payment = new TPayment
            {
                Amount = amount,
                Status = status,
                TransactionId = transactionId,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Payments.AddAsync(payment);

            // 2. 建立每筆明細
            foreach (var (compId, empId) in details)
            {
                var detail = new TPaymentDetail
                {
                    Payment = payment,
                    CompOrderId = compId,
                    EmpOrderId = empId
                };
                await _uow.PaymentDetails.AddAsync(detail);
            }

            // 3. 一次提交
            await _uow.CompleteAsync();
            return payment.PaymentId;
        }

        public async Task<TPayment?> GetByTransactionIdAsync(long transactionId)
        {
            // 直接呼叫你在 Repository 裡定義的查詢方法
            return await _uow.Payments.GetByTransactionIdAsync(transactionId);
        }

        public async Task<TPayment?> GetByIdAsync(int id)
        => await _uow.Payments.GetByIdAsync(id);          // 泛型 Repo FindAsync

        public async Task UpdateStatusAsync(int paymentId, string status)
        {
            var payment = await _uow.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException($"找不到 PaymentID={paymentId} 的付款紀錄");

            payment.Status = status;

            // ← 新增：當狀態為 "Paid" 時，填入當下時間
            if (status == "Paid")
                payment.PaidAt = DateTime.UtcNow;
            else
                payment.PaidAt = null;  // 若你想在 Failed 時清掉，也可留 NULL

            _uow.Payments.Update(payment);
            await _uow.CompleteAsync();
        }

        public async Task UpdateTransactionAsync(int paymentId, long transactionId)
        {
            var payment = await _uow.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException($"找不到 PaymentID={paymentId} 的付款紀錄");

            payment.TransactionId = transactionId;
            _uow.Payments.Update(payment);
            await _uow.CompleteAsync();
        }
    }
}
