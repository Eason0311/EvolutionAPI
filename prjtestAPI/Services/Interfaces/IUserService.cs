using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.CompanyManage;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Models.DTOs.User;
using prjEvolutionAPI.Responses;
using prjEvolutionAPI.Services;
using prjtestAPI.Models;
using prjtestAPI.Models.DTOs.Account;

namespace prjtestAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserLoginResultDTO?> ValidateUserAsync(string email, string password);
        Task<UserInfoDTO> GetUserInfoAsync(int? userId);
        Task<ServiceResult<TUser>> CreateUserAsync(RegisterEmployeeDTO dto, int callerUserId);
        Task<EditUserResponseDTO?> EditUserInfoAsync(int userId, EditUserInfoDTO dto);
        Task<IEnumerable<DepListResponseDTO>> GetDepList(int userId);
        Task<PagedResult<EmployeesListDTO>> GetClientsPagedAsync(
        int start,
        int limit,
        string sortField,
        int sortOrder,
        IDictionary<string, string> filters,
        int companyId);
        Task<int> GetUserCompanyIdAsync(int userId);
        Task<TUser?> UpdateEmployeeAsync(EmployeeUpdateDTO dto);
        Task<TUser?> UpdateStatusAsync(int userId);
        Task<int> UpdateStatusesBulkAsync(int[] userIds);
        Task<TUser?> GetByIdAsync(int userId);
    }
}
