using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface ICourseHashTagRepository
    {
        Task<TCourseHashTag> AddAsync(TCourseHashTag hashTagList);
        Task AddRangeAsync(IEnumerable<TCourseHashTag> entities);
    }
}
