using Microsoft.Extensions.Configuration;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.User;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class EmpOrderService : IEmpOrderService
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
            var courseAccess = await _uow.CourseAccesses.GetCourseAccessByUserIdAsync(userId);
            //var courseWithAccess=await _uow.Course.GetCourseByArrCourseIds(
            //    courseAccess.Select(ca => ca.CourseId).Distinct().ToList());
            var dtoListAccess = courseAccess
                .Where(a => a.Course != null && a.Course.Company != null)
                .Select(a => new EmpOrderDTO
                {
                    CourseId = (int)a.CourseId,
                    CompanyId = a.Course.CompanyId,
                    CompanyName = a.Course.Company.CompanyName ?? "",
                    CourseDes = a.Course.CourseDes ?? "",
                    CourseTitle = a.Course.CourseTitle ?? "",
                    CoverImagePath = $"{_baseUrl}/{a.Course.CoverImagePath?.TrimStart('/') ?? "noimage.png"}"
                }).ToList();


            var dtoList = orders
                .Select(o => new EmpOrderDTO
                {
                    CourseId = o.CourseId,
                    CompanyId = o.Course.CompanyId,
                    CompanyName = o.Course.Company.CompanyName!,
                    CourseDes = o.Course.CourseDes,
                    CourseTitle = o.Course.CourseTitle,
                    CoverImagePath = $"{_baseUrl}/{o.Course.CoverImagePath.TrimStart('/')}"
                }).ToList();
            // 合併兩個清單並移除重複（以 CourseId 為基準）
            var mergedList = dtoList
                .UnionBy(dtoListAccess, x => x.CourseId)
                .ToList();
            if (mergedList == null)
                return new List<EmpOrderDTO>();

            return mergedList;
        }
    }
}
