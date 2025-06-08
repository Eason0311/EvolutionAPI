using prjEvolutionAPI.Models.DTOs.Course;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PagedResult<CourseDTO>> GetPagedAsync(int pageIndex, int pageSize);
        Task<IEnumerable<CourseDTO>> GetCoursesByIdsAsync(IEnumerable<int> ids);
        Task<List<string>> GetTitleSuggestionsAsync(string prefix);
        Task<List<CourseDTO>> SearchAsync(string query);
    }
}
