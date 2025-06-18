using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface ICourseBgListRepository
    {
        Task<IEnumerable<TCourse>> GetCoursePageAsync(int CompanyId);
        Task<IEnumerable<TCourseHashTag>> GetCourseHashTagAsync(int CourseId);
        Task<IEnumerable<TCourseHashTag>> GetCourseHashTagsAsync(List<int> courseIds);
    }
}
