using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.User;
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
    }
}
