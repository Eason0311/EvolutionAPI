using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Repositories.Interfaces;
using prjEvolutionAPI.Responses;
using prjEvolutionAPI.Services.Interfaces;
using prjtestAPI.Constants;
using prjtestAPI.Helpers;
using prjtestAPI.Models;
using prjtestAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMailService _mailService;
        private readonly IUserActionTokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ICompanyRepository _repo;

        public CompanyService(
            IUnitOfWork uow,
            IMailService mailService, 
            IConfiguration configuration, 
            IUserActionTokenService tokenService,
            ICompanyRepository repo)
        {
            _uow = uow;
            _mailService = mailService;
            _tokenService = tokenService;
            _configuration = configuration;
            _repo = repo;
        }

        public async Task<ServiceResult<TCompany>> CreateCompanyWithAdminAsync(RegisterCompanyDTO dto)
        {
            var existingCompany = await _uow.Company.GetByEmailAsync(dto.Email);
            if (existingCompany != null)
                return ServiceResult<TCompany>.Fail($"聯絡 Email ({dto.Email}) 已被其他公司使用");

            var existUser = await _uow.Users.GetByEmailAsync(dto.Email);
            if (existUser != null)
                return ServiceResult<TCompany>.Fail($"此 Email ({dto.Email}) 已被其他使用者註冊");

            var newCompany = new TCompany
            {
                CompanyName = dto.CompanyName,
                CompanyEmail = dto.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = dto.IsActive,
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
                        UserStatus = "Pending",
                    };

                    _uow.Users.Add(adminUser);
                    await _uow.CompleteAsync();

                    var tokenEntity = await _tokenService.CreateTokenAsync(adminUser.UserId, UserActionTokenTypes.InitPassword, TimeSpan.FromHours(24));
                    var baseUrl = _configuration["Frontend:BaseUrl"];
                    var link = $"{baseUrl}/#/init-password?token={tokenEntity.Token}";
                    var body = EmailTemplateBuilder.BuildInitPasswordEmail(adminUser.Username, link);
                    await _mailService.SendAsync(adminUser.Email, "【學習平台】帳號啟用與密碼設定", body);
                });

                return ServiceResult<TCompany>.Success(newCompany);
            }
            catch
            {
                return ServiceResult<TCompany>.Fail("建立公司與管理員時發生錯誤，請稍後再試");
            }
        }


        public Task<PagedResult<CompanyListDTO>> GetClientsPagedAsync(
        int start, int limit, string sortField, int sortOrder,
        IDictionary<string, string> filters)
        => _repo.GetPagedAsync(start, limit, sortField, sortOrder, filters);

        public async Task<TCompany> CreateCompanyAsync(CompanyCreateDTO dto)
        {
            // 1. DTO 映射
            var entity = new TCompany
            {
                CompanyName = dto.CompanyName,
                CompanyEmail = dto.CompanyEmail,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

           _uow.Company.Add(entity);

            await _uow.CompleteAsync();

            return entity;
        }
        public async Task<TCompany?> UpdateCompanyAsync(CompanyUpdateDTO dto)
        {
            // 1. 先取出原始 entity
            var entity = await _uow.Company.GetByIdAsync(dto.CompanyId);
            if (entity == null) return null;

            // 2. 更新欄位
            entity.CompanyName = dto.CompanyName;
            entity.CompanyEmail = dto.CompanyEmail;
            entity.IsActive = dto.IsActive;
            _uow.Company.Update(entity);

            await _uow.CompleteAsync();

            return entity;
        }
    }
}
