using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.CourseBgList;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class SCourseBgListService : ICourseBgListService
    {
        private readonly IUnitOfWork _uow;
        public SCourseBgListService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IEnumerable<ResCourseBgListDTO>> GetCoursePageAsync(int userId)
        {
            var companyId = await _uow.Users.GetCompanyIdAsync(userId);
            var courses = (await _uow.CourseBgList.GetCoursePageAsync(companyId)).ToList();

            var courseIds = courses.Select(c => c.CourseId).ToList();

            var allCourseHashTags = await _uow.CourseBgList.GetCourseHashTagsAsync(courseIds);

            var groupedHashTags = allCourseHashTags
                                  .GroupBy(ht => ht.CourseId)
                                  .ToDictionary(g => g.Key, g => g.Select(ht => ht.TagId).ToList());

            var result = courses.Select(c => new ResCourseBgListDTO
            {
                CourseId = c.CourseId,
                CourseTitle = c.CourseTitle,
                IsPublic = c.IsPublic,
                CourseHashTags = groupedHashTags.ContainsKey(c.CourseId) ? groupedHashTags[c.CourseId] : new List<int>()
            });

            return result;
        }

        public async Task<IEnumerable<ResUserBgListDTO>> GetEmployeeAsync(int userId)
        {
            var companyId = await _uow.Users.GetCompanyIdAsync(userId);
            var userList = await _uow.Users.GetEmployeesByCompanyIdAsync(companyId);
            var ResUserList =userList.Select(u => new ResUserBgListDTO
            {
                UserId = u.UserId,
                UserName = u.Username,
                UserEmail = u.Email,
                DepId = u.UserDep
            });
            return ResUserList;
        }
    }
}
