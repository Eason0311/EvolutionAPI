using prjEvolutionAPI.Models.DTOs.Course;
using prjEvolutionAPI.Models.DTOs.Home;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface IHomeService
    {
        Task<AboutInfoDTO> GetAboutInfoAsync();
        Task<List<HashTagListDTO>> GetRandomTagAsync();
    }
}
