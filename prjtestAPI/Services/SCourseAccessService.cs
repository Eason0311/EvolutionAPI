using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class SCourseAccessService : ICourseAccessService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SCourseAccessService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> CreateCourseAccessAsync(VCourseAccessDTO dto)
        {
            if (dto == null || dto.DepIds == null || dto.DepIds.Length == 0)
            {
                return false;
            }

            var users = await _unitOfWork.Users.GetUsersByDepartmentsAsync(dto.DepIds);

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

    }
}
