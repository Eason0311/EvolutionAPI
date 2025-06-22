using prjEvolutionAPI.Models.DTOs.CreateCourse;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface IDepListService
    {
        Task<IEnumerable<ResDepListDTO>> GetAllDepsAsync(int userId);
    }
}
