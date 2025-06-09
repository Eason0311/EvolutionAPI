using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Models.DTOs.User;
using prjEvolutionAPI.Responses;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<ServiceResult> CreateCompanyWithAdminAsync(RegisterCompanyDTO dto);
        Task<PagedResult<CompanyListDTO>> GetClientsPagedAsync(
        int start,
        int limit,
        string sortField,
        int sortOrder,
        IDictionary<string, string> filters);
    }
}
