using prjEvolutionAPI.Models;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly EvolutionApiContext _context;
        public CompanyRepository(Models.EvolutionApiContext context)
        {
            _context = context;
        }
        public void Add(TCompany company)
        {
            _context.TCompanies.Add(company);
        }
        public async Task<TCompany?> GetByIdAsync(int companyId)
        {
            return await _context.TCompanies.FindAsync(companyId);
        }
        public async Task<TCompany?> GetByEmailAsync(string email)
        {
            return _context.TCompanies.FirstOrDefault(c => c.CompanyEmail == email);
        }

    }
}
