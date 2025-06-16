using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<int> CreatePaymentAsync(
       decimal amount,
       string status,
       long? transactionId,
       IEnumerable<(int? compOrderId, int? empOrderId)> details);
        Task<TPayment?> GetByTransactionIdAsync(long transactionId);
        Task<TPayment?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int paymentId, string status);
        Task UpdateTransactionAsync(int paymentId, long transactionId);
        Task MarkPaymentAsPaidAsync(long transactionId);
    }
}
