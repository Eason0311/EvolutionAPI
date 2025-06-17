using Microsoft.Extensions.Configuration;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.User;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class EmpOrderService: IEmpOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly string _baseUrl;
        IConfiguration _configuration;
        public EmpOrderService(IUnitOfWork uow, IConfiguration configuration)
        {
            _uow = uow;
            _configuration = configuration;

            _baseUrl = _configuration.GetValue<string>("AppSettings:BaseUrl")?.TrimEnd('/')
                 ?? throw new InvalidOperationException("AppSettings:BaseUrl 未設定");
        }
        public async Task<IEnumerable<EmpOrderDTO>> GetEmpOrderListByIdAsync(int userId)
        {
            var chechUserId = await _uow.Users.GetByIdAsync(userId);
            if (chechUserId == null)
                throw new KeyNotFoundException($"找不到 User (UserID = {userId})");

            var orders = await _uow.EmpOrder.GetByEmployeeIdAsync(userId);
            if (orders == null || !orders.Any())
                return new List<EmpOrderDTO>();

            var dtoList = orders
                .Select(o => new EmpOrderDTO
                {
                    CourseId = o.CourseId,
                    CompanyId = o.Course.CompanyId,
                    CompanyName = o.Course.Company.CompanyName!,
                    CourseDes = o.Course.CourseDes,
                    CourseTitle = o.Course.CourseTitle,
                    CoverImagePath = $"{_baseUrl}/images/{o.Course.CoverImagePath.TrimStart('/')}"
                }).ToList();

            return dtoList;
        }
    }
}
