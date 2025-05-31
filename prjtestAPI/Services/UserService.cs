using prjtestAPI.Models;
using prjtestAPI.Models.DTOs.Account;
using prjtestAPI.Repositories.Interfaces;
using prjtestAPI.Services.Interfaces;

namespace prjtestAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepo, IPasswordHasher passwordHasher)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserLoginResultDTO> ValidateUserAsync(string email, string password)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null)
            {
                return new UserLoginResultDTO
                {
                    EmailExists = false,
                    PasswordValid = false,
                    User = null
                };
            }

            bool isPasswordValid = _passwordHasher.Verify(password, user.PasswordHash);

            return new UserLoginResultDTO
            {
                EmailExists = true,
                PasswordValid = isPasswordValid,
                User = user // 無論正確與否都回傳 user
            };
        }
    }
}
