using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Models.DTOs.User;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface ICompenyService
    {
        Task<ServiceResult> CreateCompanyWithAdminAsync(RegisterCompanyDTO dto);
    }
}
