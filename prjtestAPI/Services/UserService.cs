using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Models.DTOs.User;
using prjEvolutionAPI.Services;
using prjtestAPI.Constants;
using prjtestAPI.Helpers;
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
        private readonly IUnitOfWork _uow;
        private readonly IMailService _mailService;
        private readonly IUserActionTokenService _tokenService;
        private readonly IConfiguration _configuration;

        public UserService(
            IUserRepository userRepo,
            IPasswordHasher passwordHasher,
            IUnitOfWork uow,
            IMailService mailService,
            IConfiguration configuration,
            IUserActionTokenService tokenService)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _uow = uow;
            _mailService = mailService;
            _tokenService = tokenService;
            _configuration = configuration;
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

        public async Task<UserInfoDTO> GetUserInfoAsync(int? userId)
        {
            if (userId == null)
                return null;

            TUser? user = await _userRepo.GetByIdAsync(userId.Value);
            if (user == null)
                return null;

            return new UserInfoDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email
            };
        }

        public async Task<ServiceResult> CreateUserAsync(RegisterEmployeeDTO dto , int callerUserId)
        {
            var caller = await _uow.Users.GetByIdAsync(callerUserId);
            if (caller == null)
                return ServiceResult.Fail("找不到管理員帳號");

            var existUser = await _uow.Users.GetByEmailAsync(dto.Email);
            if (existUser != null)
                return ServiceResult.Fail($"此 Email ({dto.Email}) 已被其他使用者註冊");

            int newEmployeeCompanyId = caller.CompanyId;
            string rawPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
            string hashedPassword = PasswordHasher.Hash(rawPassword);

            try 
            {
                int newEmployeeUserDep = 0;

                await _uow.ExecuteTransactionAsync(async () =>
                {
                    var existDept = await _uow.DepList.GetFirstOrDefaultAsync(dto.DepName, newEmployeeCompanyId);

                    TDepList deptEntity;
                    if (existDept == null)
                    {
                        deptEntity = new TDepList
                        {
                            CompanyId = newEmployeeCompanyId,
                            DepName = dto.DepName
                        };
                        _uow.DepList.Add(deptEntity);
                        await _uow.CompleteAsync(); 

                        newEmployeeUserDep = deptEntity.DepId;
                    }
                    else
                    {
                        deptEntity = existDept;
                        newEmployeeUserDep = existDept.DepId;
                    }

                    var newUser = new TUser
                    {
                        Username = dto.Username,
                        Email = dto.Email,
                        PasswordHash = hashedPassword,
                        Role = "User",
                        IsEmailConfirmed = false,
                        CompanyId = newEmployeeCompanyId,
                        CreatedAt = DateTime.UtcNow,
                        UserDep = newEmployeeUserDep,
                        UserStatus = "Active",
                    };

                    _uow.Users.Add(newUser);
                    await _uow.CompleteAsync();

                    var tokenEntity = await _tokenService.CreateTokenAsync(newUser.UserId, UserActionTokenTypes.InitPassword, TimeSpan.FromHours(24));
                    var baseUrl = _configuration["Frontend:BaseUrl"]; // 注入 IConfiguration
                    var link = $"{baseUrl}/#/init-password?token={tokenEntity.Token}";
                    var body = EmailTemplateBuilder.BuildInitPasswordEmail(newUser.Username, link);
                    await _mailService.SendAsync(newUser.Email, "【學習平台】帳號啟用與密碼設定", body);
                });

                return ServiceResult.Success();

            }
            catch 
            {
                return ServiceResult.Fail("建立員工帳號時發生錯誤，請稍後再試");
            }
        }
    }
}
