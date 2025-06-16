using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class PaymentRepository : Repository<TPayment>,IPaymentRepository
    {
        public PaymentRepository(EvolutionApiContext context) : base(context) { }

        public async Task<TPayment?> GetByTransactionIdAsync(long transactionId)
        => await _dbSet
             .Include(p => p.TPaymentDetails)
             .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
    }
}
