using Microsoft.EntityFrameworkCore;
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
        public async Task<IEnumerable<TCourseAccess>> GetCourseAccessByCourseIdAsync(int courseId)
        {
            if (courseId <= 0)
                return Enumerable.Empty<TCourseAccess>();
            return await _context.Set<TCourseAccess>()
                .Where(ca => ca.CourseId == courseId)
                .ToListAsync();
        }
        public async Task<TCourseAccess> AddCourseAsync(TCourseAccess courseAccess)
        {

            var entity = await _context.Set<TCourseAccess>().AddAsync(courseAccess);
            await _context.SaveChangesAsync();
            return entity.Entity;
        }
        public async Task<TCourseAccess> GetByIdAsync(int courseAccessId)
        {
            if (courseAccessId <= 0)
                return null;
            return await _context.Set<TCourseAccess>()
                .FirstOrDefaultAsync(ca => ca.CourseAccessId == courseAccessId);
        }
        public async Task Remove(TCourseAccess courseAccess)
        {
            if (courseAccess == null)
                throw new ArgumentNullException(nameof(courseAccess), "Cannot remove null courseAccess entity.");

             _context.Set<TCourseAccess>().Remove(courseAccess);
            // 此處不呼叫 SaveChangesAsync，統一由 UnitOfWork 控制交易
        }

    }
}
