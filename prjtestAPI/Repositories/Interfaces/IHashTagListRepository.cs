using prjEvolutionAPI.Models.DTOs.Course;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface IHashTagListRepository
    {
        Task<List<HashTagListDTO>> GetRandomTagAsync(int count = 3);
    }
}
