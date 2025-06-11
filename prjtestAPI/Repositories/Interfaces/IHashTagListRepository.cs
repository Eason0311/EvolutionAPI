using prjEvolutionAPI.Models.DTOs.Course;
using prjEvolutionAPI.Models.DTOs.CreateCourse;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface IHashTagListRepository
    {
        Task<List<HashTagListDTO>> GetRandomTagAsync(int count = 3);
        Task<IEnumerable<ResHashTagDTO>> GetAllHashTagsAsync();
    }
}
