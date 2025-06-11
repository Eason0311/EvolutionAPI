using prjEvolutionAPI.Models.DTOs.CreateCourse;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface ICourseAccessService
    {
        Task<bool> CreateCourseAccessAsync(VCourseAccessDTO dto);
    }
}
