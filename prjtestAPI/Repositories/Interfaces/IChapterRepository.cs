using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface IChapterRepository
    {
        Task<TCourseChapter> GetByIdAsync(int id);
        Task<IEnumerable<TCourseChapter>> GetAllAsync();
        Task<TCourseChapter> AddAsync(TCourseChapter chapter);
        void Update(TCourseChapter chapter);
        void Delete(TCourseChapter chapter);
    }
}
