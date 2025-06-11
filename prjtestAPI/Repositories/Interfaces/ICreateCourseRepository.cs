using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface ICreateCourseRepository
    {
        Task<TCourse> GetByIdAsync(int id);
        Task<IEnumerable<TCourse>> GetAllAsync();
        Task<TCourse> AddAsync(TCourse course);
        void Update(TCourse course);
        void Delete(TCourse course);
    }
}
