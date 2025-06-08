using Microsoft.Extensions.Caching.Memory;
using prjEvolutionAPI.Models.DTOs.Course;
using prjEvolutionAPI.Repositories.Interfaces;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMemoryCache _cache;

        public CourseService(ICourseRepository repo, IUnitOfWork uow, IMemoryCache cache)
        {
            _repo = repo;
            _uow = uow;
            _cache = cache;
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

        public async Task<List<string>> GetTitleSuggestionsAsync(string prefix)
        {
            if (prefix.Length < 2)
                return new List<string>();

            var key = $"Suggest_{prefix}";
            if (!_cache.TryGetValue(key, out List<string> list))
            {
                list = await _repo.GetTitleSuggestionsAsync(prefix, 10);
                _cache.Set(key, list, TimeSpan.FromMinutes(2));
            }
            return list;
        }

        public async Task<List<CourseDTO>> SearchAsync(string query)
        {
            // 可以不快取，亦可依需求加入
            return await _repo.SearchAsync(query);
        }
    }
}
