using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface IVideoRepository
    {
        Task<TVideo> GetByIdAsync(int id);
        Task<IEnumerable<TVideo>> GetAllAsync();
        Task<TVideo> AddAsync(TVideo video);
        void Update(TVideo video);
        void Delete(TVideo video);
    }
}
