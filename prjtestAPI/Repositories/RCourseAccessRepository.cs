using prjEvolutionAPI.Models;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class RCourseAccessRepository : ICourseAccessRepository
    {
        private readonly EvolutionApiContext _context;
        public RCourseAccessRepository(EvolutionApiContext context)
        {
            _context = context;
        }
        public async Task AddRangeAsync(IEnumerable<TCourseAccess> entities)
        {
            if (entities == null || !entities.Any())
                return;
            await _context.Set<TCourseAccess>().AddRangeAsync(entities);
        }
    }
}
