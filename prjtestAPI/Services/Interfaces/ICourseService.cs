using Microsoft.AspNetCore.SignalR;
using prjEvolutionAPI.Hubs;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Course;
using prjEvolutionAPI.Models.DTOs.CreateCourse;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PagedResult<CourseDTO>> GetPagedAsync(int pageIndex, int pageSize);
        Task<IEnumerable<CourseDTO>> GetCoursesByIdsAsync(IEnumerable<int> ids);
        Task<List<string>> GetTitleSuggestionsAsync(string prefix);
        Task<List<CourseDTO>> SearchAsync(string query);
        Task<int> CreateCourseAsync(VCourseDTO dto, string UserId,int CompanyId, IHubContext<CourseHub> hubContext);
        Task<TCourse> GetCourseAsync(int id);
        Task<IEnumerable<TCourse>> GetAllCoursesAsync();
        Task<bool> UpdateCourseAsync(int id, VCourseDTO dto, string ConnectionId, IHubContext<CourseHub> hubContext);
        Task<bool> DeleteCourseAsync(int id);
        Task<bool> MarkCourseAsCompletedAsync(int id, VFinalDTO finalDTO, IHubContext<CourseHub> hubContext);
        Task<ResFinalCourse> GetCourseLearning(int courseId);
    }
}
