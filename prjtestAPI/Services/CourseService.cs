using prjEvolutionAPI.Models.DTOs.Course;
using prjEvolutionAPI.Repositories.Interfaces;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repo;
        private readonly IUnitOfWork _uow;

        public CourseService(ICourseRepository repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<PagedResult<CourseDTO>> GetPagedAsync(int pageIndex, int pageSize)
        {
            var (itemsDto, totalCount) = await _repo.GetPagedAsync(pageIndex, pageSize);

            // 2. 直接把這個 DTO 清單丟到 PagedResult 裡
            return new PagedResult<CourseDTO>
            {
                Items = itemsDto.ToList(),  // or just itemsDto if PagedResult.Items is IEnumerable<T>
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public Task<IEnumerable<CourseDTO>> GetCoursesByIdsAsync(IEnumerable<int> ids)
    => _repo.GetByIdsAsync(ids);
    }
}
