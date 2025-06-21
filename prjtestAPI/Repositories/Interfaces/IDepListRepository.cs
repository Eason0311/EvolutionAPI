using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.CreateCourse;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface IDepListRepository
    {
        Task<TDepList?> GetFirstOrDefaultAsync(string depName, int companyId);
        Task<TDepList?> GetByIdAsync(int UserDe);
        Task<IEnumerable<TDepList?>> GetByCompanyIdAsync(int companyId);
        Task AddAsync(TDepList depList);
        Task<IEnumerable<ResDepListDTO>> GetAllDepartmentsAsync(int companyId);
    }
}
