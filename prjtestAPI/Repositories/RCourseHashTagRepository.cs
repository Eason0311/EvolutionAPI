using prjEvolutionAPI.Models;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class RCourseHashTagRepository : ICourseHashTagRepository
    {
        private readonly EvolutionApiContext _context;
        public RCourseHashTagRepository(EvolutionApiContext context)
        {
            _context = context;
        }

        public async Task<TCourseHashTag> AddAsync(TCourseHashTag hashTagList)
        {
            var entry = await _context.TCourseHashTags.AddAsync(hashTagList);
            return entry.Entity; // ✅ 回傳資料本體
        }
        public async Task AddRangeAsync(IEnumerable<TCourseHashTag> entities)
        {
            await _context.Set<TCourseHashTag>().AddRangeAsync(entities);
        }
    }
}
