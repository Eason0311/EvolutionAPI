using prjEvolutionAPI.Models.DTOs.Publisher;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface IPublisherService
    {
        Task<List<CompanyListDTO>> GetCompanyListAsync();
    }
}
