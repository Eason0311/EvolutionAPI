using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface ICourseAccessRepository
    {
        Task AddRangeAsync(IEnumerable<TCourseAccess> entities);
        Task<IEnumerable<TCourseAccess>> GetCourseAccessByCourseIdAsync(int courseId);
        Task<TCourseAccess> AddCourseAsync(TCourseAccess courseAccess);
        Task<TCourseAccess> GetByIdAsync(int courseAccessId);
        Task Remove(TCourseAccess courseAccess);
        Task<IEnumerable<TCourseAccess>> GetCourseAccessByUserIdAsync(int userId);
    }
}
