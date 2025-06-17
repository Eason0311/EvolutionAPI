using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Services.Interfaces;
using prjEvolutionAPI.Models.DTOs.CourseBgList;

namespace prjEvolutionAPI.Services
{
    public class SCourseAccessService : ICourseAccessService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SCourseAccessService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> CreateCourseAccessAsync(VCourseAccessDTO dto, int userId)
        {
            if (dto == null || dto.DepIds == null || dto.DepIds.Length == 0)
            {
                return false;
            }

            var users = await _unitOfWork.Users.GetUsersByDepartmentsAsync(dto.DepIds, userId);

            if (users == null || !users.Any())
            {
                return false; // 沒有符合的使用者
            }

            var entities = users.Select(user => new TCourseAccess
            {
                CourseId = dto.courseId,
                UserId = user.UserId
            });

            await _unitOfWork.CourseAccesses.AddRangeAsync(entities);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        public async Task<IEnumerable<TCourseAccess[]>> GetCourseAccessAsync(int courseId)
        {

            if (courseId <= 0)
            {
                return Enumerable.Empty<TCourseAccess[]>();
            }
            var courseAccesses = await _unitOfWork.CourseAccesses.GetCourseAccessByCourseIdAsync(courseId);
            if (courseAccesses == null || !courseAccesses.Any())
            {
                return Enumerable.Empty<TCourseAccess[]>();
            }
            return courseAccesses.GroupBy(ca => ca.CourseId)
                                 .Select(g => g.ToArray());
        }
        public async Task<int> AddCourseAccessAsync(VAddCourseAccessDTO dto)
        {
            
            var courseAccess = new TCourseAccess
            {
                UserId = dto.UserId,
                CourseId = dto.CourseId
            };
            var entity=await _unitOfWork.CourseAccesses.AddCourseAsync(courseAccess);
            await _unitOfWork.CompleteAsync();
            return entity.CourseAccessId;
        }
        public async Task<bool> DelUserAccess(int courseAccessId)
        {
            if (courseAccessId <= 0)
            {
                return false;
            }
            var courseAccess = await _unitOfWork.CourseAccesses.GetByIdAsync(courseAccessId);
            if (courseAccess == null)
            {
                return false; // 找不到指定的課程存取
            }
            await _unitOfWork.CourseAccesses.Remove(courseAccess);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
