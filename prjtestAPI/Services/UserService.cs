using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Account;
using prjEvolutionAPI.Models.DTOs.CompanyManage;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Models.DTOs.User;
using prjEvolutionAPI.Repositories.Interfaces;
using prjEvolutionAPI.Responses;
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
        private readonly IWebHostEnvironment _env;
        private readonly IJwtService _jwtService;
        private readonly string _baseUrl;
        private readonly IEmpOrderRepository _empOrderRepo;

        public UserService(
            IUserRepository userRepo,
            IPasswordHasher passwordHasher,
            IUnitOfWork uow,
            IMailService mailService,
            IConfiguration configuration,
            IWebHostEnvironment env,
            IUserActionTokenService tokenService,
            IJwtService jwtService,
            IEmpOrderRepository empOrderRepo)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _uow = uow;
            _mailService = mailService;
            _tokenService = tokenService;
            _configuration = configuration;
            _env = env;
            _jwtService = jwtService;
            _empOrderRepo = empOrderRepo;
            _baseUrl = _configuration.GetValue<string>("AppSettings:BaseUrl")?.TrimEnd('/')
                 ?? throw new InvalidOperationException("AppSettings:BaseUrl 未設定");
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

        public async Task<UserInfoDTO?> GetUserInfoAsync(int? userId)
        {
            if (userId == null)
                return null;

            TUser? user = await _userRepo.GetByIdWithDepAsync(userId.Value);
            if (user == null)
                return null;

            string companyName = user.Company?.CompanyName ?? string.Empty;
            string depName = user.UserDepNavigation?.DepName ?? string.Empty;

            // 如果未上傳過照片，user.UserPic 可能為 null 或空字串
            string? photoUrl = null;
            if (!string.IsNullOrEmpty(user.UserPic))
            {
                // user.UserPic 例如 "uploads/users/xxx.jpg"
                photoUrl = $"{_baseUrl}/{user.UserPic.TrimStart('/')}";
            }

            return new UserInfoDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                UserCompany = user.Company.CompanyName,
                Email = user.Email,
                UserPicPath = user.UserPic,
                DepName = user.UserDepNavigation.DepName,
                PhotoUrl = photoUrl
            };
        }

        public async Task<ServiceResult<TUser>> CreateUserAsync(RegisterEmployeeDTO dto, int callerUserId)
        {
            var caller = await _uow.Users.GetByIdAsync(callerUserId);
            if (caller == null)
                return ServiceResult<TUser>.Fail("找不到管理員帳號");

            var existUser = await _uow.Users.GetByEmailAsync(dto.Email);
            if (existUser != null)
                return ServiceResult<TUser>.Fail($"此 Email ({dto.Email}) 已被其他使用者註冊");

            int newEmployeeCompanyId = caller.CompanyId;
            string rawPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
            string hashedPassword = PasswordHasher.Hash(rawPassword);

            TUser newUser = null!;

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
                        await _uow.DepList.AddAsync(deptEntity);
                        await _uow.CompleteAsync();

                        newEmployeeUserDep = deptEntity.DepId;
                    }
                    else
                    {
                        deptEntity = existDept;
                        newEmployeeUserDep = existDept.DepId;
                    }

                    newUser = new TUser
                    {
                        Username = dto.Username,
                        Email = dto.Email,
                        PasswordHash = hashedPassword,
                        Role = "User",
                        IsEmailConfirmed = false,
                        CompanyId = newEmployeeCompanyId,
                        CreatedAt = DateTime.UtcNow,
                        UserDep = newEmployeeUserDep,
                        UserStatus = "Pending",
                    };

                    _uow.Users.Add(newUser);
                    await _uow.CompleteAsync();

                    var tokenEntity = await _tokenService.CreateTokenAsync(newUser.UserId, UserActionTokenTypes.InitPassword, TimeSpan.FromHours(24));
                    var baseUrl = _configuration["Frontend:BaseUrl"];
                    var link = $"{baseUrl}/#/init-password?token={tokenEntity.Token}";
                    var body = EmailTemplateBuilder.BuildInitPasswordEmail(newUser.Username, link);
                    await _mailService.SendAsync(newUser.Email, "【學習平台】帳號啟用與密碼設定", body);
                });

                return ServiceResult<TUser>.Success(newUser);
            }
            catch
            {
                return ServiceResult<TUser>.Fail("建立員工帳號時發生錯誤，請稍後再試");
            }
        }

        public async Task<EditUserResponseDTO?> EditUserInfoAsync(int userId, EditUserInfoDTO dto)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null)
                return null;

            var checkEmail = await _uow.Users.GetByEmailAsync(dto.UserEmail);
            if (checkEmail != null && checkEmail.UserId != userId)
                return null;

            user.Username = dto.Username;
            user.Email = dto.UserEmail;

            if(!string.IsNullOrWhiteSpace(dto.Department))
            {
                var dept = await _uow.DepList.GetFirstOrDefaultAsync(dto.Department.Trim(), user.CompanyId);
                if (dept != null)
                    user.UserDep = dept.DepId;
                else
                {
                    TDepList deptEntity = new TDepList
                    {
                        CompanyId = user.CompanyId,
                        DepName = dto.Department
                    };
                    await _uow.DepList.AddAsync(deptEntity);
                    await _uow.CompleteAsync();

                    user.UserDep = deptEntity.DepId;
                }
            }

            string? relativePath = null;
            if (dto.PhotoFile != null && dto.PhotoFile.Length > 0)
            {
                // 5.1 確保 uploads/users 資料夾存在
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "users");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // 5.2 產生新的檔名 (GUID + 原始副檔名)
                var ext = Path.GetExtension(dto.PhotoFile.FileName); // 例 ".jpg"
                var newFileName = $"{Guid.NewGuid()}{ext}";
                var physicalPath = Path.Combine(uploadsFolder, newFileName);

                // 5.3 將檔案寫入硬碟
                using (var fs = new FileStream(physicalPath, FileMode.Create))
                    await dto.PhotoFile.CopyToAsync(fs);

                // 5.4 刪除舊照片 (如果存在)
                if (!string.IsNullOrEmpty(user.UserPic))
                {
                    var oldPhysical = Path.Combine(_env.WebRootPath,
                        user.UserPic.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (File.Exists(oldPhysical))
                    {
                        try { File.Delete(oldPhysical); }
                        catch { /* 忽略刪除失敗 */ }
                    }
                }

                // 5.5 更新 user 的相對路徑欄位
                user.UserPic = Path.Combine("uploads", "users", newFileName)
                    .Replace("\\", "/");

                // 5.6 設定回傳給 Controller 的相對路徑
                relativePath = user.UserPic;
            }

            // 6. 標記 User 更新，並 Commit (此時 Dept 的新增若有也已 commit)
            _uow.Users.Update(user);
            await _uow.CompleteAsync();

            // 7. 若前面沒上傳新照片，但 user.UserPic 原本就有值，則也把它放入 relativePath
            if (relativePath == null && !string.IsNullOrEmpty(user.UserPic))
                relativePath = user.UserPic;

            // 8. 簽發新的 Access Token (JWT)，把最新的 username、role...都放進去
            var newJwt = _jwtService.GenerateAccessToken(user);

            // 9. 回傳給 Controller 的 DTO
            return new EditUserResponseDTO
            {
                UserInfo = new UserInfoDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    DepName = user.UserDepNavigation.DepName, // 確保 UserDepNavigation 已 load
                    Email = user.Email,
                    UserPicPath = relativePath  // Controller 補成完整 PhotoUrl
                },
                NewAccessToken = newJwt
            };
        }

        public async Task<IEnumerable<DepListResponseDTO>> GetDepList(int userId)
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null)
                return Enumerable.Empty<DepListResponseDTO>();

            var depList = await _uow.DepList.GetByCompanyIdAsync(user.CompanyId);
            var dtoList = depList
                .Where(d => d != null)
                .Select(d => new DepListResponseDTO { DepName = d!.DepName })
                .ToList();
            return dtoList;
        }

        public Task<PagedResult<EmployeesListDTO>> GetClientsPagedAsync(
        int start, int limit, string sortField, int sortOrder,
        IDictionary<string, string> filters, int companyId)
        => _userRepo.GetPagedAsync(start, limit, sortField, sortOrder, filters , companyId);

        public async Task<int> GetUserCompanyIdAsync(int userId)
        {
            return await _uow.Users.GetCompanyIdAsync(userId);
        }

        public async Task<TUser?> UpdateEmployeeAsync(EmployeeUpdateDTO dto)
        {
            // 1. 取出原始使用者實體
            var user = await _uow.Users.GetByIdAsync(dto.UserId);
            if (user == null)
                return null;

            // 2. 依部門名稱 + 公司 ID 嘗試取得已存在的部門
            var department = await _uow.DepList
                .GetFirstOrDefaultAsync(dto.UserDep,user.CompanyId);

            // 3. 若不存在，建立新部門實體
            if (department == null)
            {
                department = new TDepList
                {
                    DepName = dto.UserDep,
                    CompanyId = user.CompanyId
                };
                // 如果你的 Repository 支援 AddAsync，優先用 AddAsync
                await _uow.DepList.AddAsync(department);
            }

            // 4. 更新使用者欄位
            user.Username = dto.Username;
            user.Email = dto.Email;
            user.UserDep = department.DepId;   // 假設外鍵欄位是 UserDep

            // 5. 標記更新（若為 EF Core，這行可省略，因為 ChangeTracker 已追蹤 user）
            _uow.Users.Update(user);

            // 6. 最後一次儲存、提交整個事務
            await _uow.CompleteAsync();

            return user;
        }

        public async Task<TUser?> UpdateStatusAsync(int userId, string newStatus)
        {
            // 1. 先撈到 user 本體
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null) return null;

            // 2. 更新狀態
            user.UserStatus = newStatus;
            await _uow.CompleteAsync();

            // 3. 再去撈 department entity
            var dep = await _uow.DepList.GetByIdAsync(user.UserDep);
            user.UserDepNavigation = dep;   // 把 navigation property 塞回去

            return user;
        }
    }
}