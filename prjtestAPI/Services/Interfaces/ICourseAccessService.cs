using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.CourseBgList;
using prjEvolutionAPI.Models.DTOs.CreateCourse;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface ICourseAccessService
    {
        Task<bool> CreateCourseAccessAsync(VCourseAccessDTO dto,int userId);
        Task<IEnumerable<TCourseAccess[]>> GetCourseAccessAsync(int courseId);
        Task<int> AddCourseAccessAsync(VAddCourseAccessDTO dto);
        Task<bool> DelUserAccess(int courseAccessId);

    }
}
