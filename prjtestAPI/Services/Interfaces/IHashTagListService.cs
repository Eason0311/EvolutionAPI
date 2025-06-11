using prjEvolutionAPI.Models.DTOs.CreateCourse;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface IHashTagListService
    {
        Task<IEnumerable<ResHashTagDTO>> GetAllHashTagsAsync();
    }
}
