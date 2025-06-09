using prjEvolutionAPI.Models.DTOs.Publisher;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface IPublisherRepository
    {
        Task<List<CompanyListDTO>> GetAllAsync();
    }
}
