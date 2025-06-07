using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class EmpOrderRepository: IEmpOrderRepository
    {
        private readonly EvolutionApiContext _context;
        public EmpOrderRepository(EvolutionApiContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TEmpOrder>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.TEmpOrders
                .AsNoTracking()
                .Where(e => e.BuyerUserId == employeeId)
                .Include(e => e.Course)
                .ThenInclude(c => c.Company)
                .ToListAsync();
        }
    }
}
