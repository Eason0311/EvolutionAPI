using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Services.Interfaces;
using prjtestAPI.Constants;
using prjtestAPI.Helpers;
using prjtestAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMailService _mailService;
        private readonly IUserActionTokenService _tokenService;
        private readonly IConfiguration _configuration;

        public CompanyService(
            IUnitOfWork uow,
            IMailService mailService, 
            IConfiguration configuration, 
            IUserActionTokenService tokenService)
        {
            _uow = uow;
            _mailService = mailService;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<ServiceResult> CreateCompanyWithAdminAsync(RegisterCompanyDTO dto)
        {
            var existingCompany = await _uow.Company.GetByEmailAsync(dto.Email);
            if (existingCompany != null)
                return ServiceResult.Fail($"聯絡 Email ({dto.Email}) 已被其他公司使用");

            var existUser = await _uow.Users.GetByEmailAsync(dto.Email);
            if (existUser != null)
                return ServiceResult.Fail($"此 Email ({dto.Email}) 已被其他使用者註冊");

            var newCompany = new TCompany
            {
                CompanyName = dto.CompanyName,
                CompanyEmail = dto.Email,
                CreatedAt = DateTime.UtcNow,
            };

            string rawPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
            string hashedPassword = PasswordHasher.Hash(rawPassword);

            try
            {
                await _uow.ExecuteTransactionAsync(async () =>
                {
                    _uow.Company.Add(newCompany);
                    await _uow.CompleteAsync();

                    var defaultDept = new TDepList
                    {
                        DepName = "管理員",
                        CompanyId = newCompany.CompanyId
                    };
                    _uow.DepList.Add(defaultDept);
                    await _uow.CompleteAsync();

                    var adminUser = new TUser
                    {
                        Username = dto.CompanyName,
                        Email = dto.Email,
                        PasswordHash = hashedPassword,
                        Role = "Admin",
                        IsEmailConfirmed = false,
                        CompanyId = newCompany.CompanyId,
                        CreatedAt = DateTime.UtcNow,
                        UserDep = defaultDept.DepId,
                        UserStatus = "Active",
                    };

                    _uow.Users.Add(adminUser);
                    await _uow.CompleteAsync();

                    var tokenEntity = await _tokenService.CreateTokenAsync(adminUser.UserId, UserActionTokenTypes.InitPassword, TimeSpan.FromHours(24));
                    var baseUrl = _configuration["Frontend:BaseUrl"]; // 注入 IConfiguration
                    var link = $"{baseUrl}/#/init-password?token={tokenEntity.Token}";
                    var body = EmailTemplateBuilder.BuildInitPasswordEmail(adminUser.Username, link);
                    await _mailService.SendAsync(adminUser.Email, "【學習平台】帳號啟用與密碼設定", body);
                });

                return ServiceResult.Success();
            }
            catch
            {
                return ServiceResult.Fail("建立公司與管理員時發生錯誤，請稍後再試");
            }
        }
    }
}
