using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class SCourseHashTagService : ICourseHashTagService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SCourseHashTagService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> CreateCourseHashTag(VCourseHashTag dto)
        {
            if (dto == null || dto.hashTagIds == null || dto.hashTagIds.Length == 0)
                return false;

            var entities = dto.hashTagIds.Select(tagId => new TCourseHashTag
            {
                CourseId = dto.courseId,
                TagId = tagId
            });

            await _unitOfWork.CourseHashTags.AddRangeAsync(entities);
            await _unitOfWork.CompleteAsync();
            return true;
        }

    }
}
