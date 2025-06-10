using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface IDepListRepository
    {
        Task<TDepList?> GetFirstOrDefaultAsync(string depName, int companyId);
        Task<TDepList?> GetByIdAsync(int UserDe);
        Task<IEnumerable<TDepList?>> GetByCompanyIdAsync(int companyId);
        Task AddAsync(TDepList depList);
    }
}
