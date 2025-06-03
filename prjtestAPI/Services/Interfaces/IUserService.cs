using prjEvolutionAPI.Models.DTOs;
using prjtestAPI.Models;
using prjtestAPI.Models.DTOs.Account;

namespace prjtestAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserLoginResultDTO?> ValidateUserAsync(string email, string password);
        Task<UserInfoDTO> GetUserInfoAsync(int? userId);
    }
}
