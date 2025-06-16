using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface IPaymentRepository : IRepository<TPayment>
    {
        Task<TPayment?> GetByTransactionIdAsync(long transactionId);
    }
}
