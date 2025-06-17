using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Models.DTOs.User;
using prjEvolutionAPI.Responses;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<ServiceResult<TCompany>> CreateCompanyWithAdminAsync(RegisterCompanyDTO dto);
        Task<PagedResult<CompanyListDTO>> GetClientsPagedAsync(
        int start,
        int limit,
        string sortField,
        int sortOrder,
        IDictionary<string, string> filters);
        Task<TCompany> CreateCompanyAsync(CompanyCreateDTO dto);
        Task<TCompany?> UpdateCompanyAsync(CompanyUpdateDTO dto);
        Task<TCompany?> GetByIdAsync(int compId);
    }
}
