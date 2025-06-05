using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface IDepListRepository
    {
        Task<TDepList?> GetFirstOrDefaultAsync(string depName, int companyId);
        void Add(TDepList depList);
    }
}
