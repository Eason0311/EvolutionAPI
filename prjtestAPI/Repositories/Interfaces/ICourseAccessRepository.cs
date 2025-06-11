using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface ICourseAccessRepository
    {
        Task AddRangeAsync(IEnumerable<TCourseAccess> entities);
    }
}
