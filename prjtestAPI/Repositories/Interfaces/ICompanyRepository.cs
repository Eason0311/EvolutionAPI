using prjEvolutionAPI.Models;
using prjtestAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        void Add(TCompany company);
        Task<TCompany?> GetByIdAsync(int companyId);
        Task<TCompany?> GetByUserIdAsync(int userId);
        Task<TCompany?> GetByEmailAsync(string email);
        Task<int> GetCompanyCountAsync();
    }
}
