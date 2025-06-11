using prjEvolutionAPI.Models.DTOs.CreateCourse;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface ICourseHashTagService
    {
        Task<bool> CreateCourseHashTag(VCourseHashTag dto);
    }
}
