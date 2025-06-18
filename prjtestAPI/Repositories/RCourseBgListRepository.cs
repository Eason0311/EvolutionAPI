using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class RCourseBgListRepository : ICourseBgListRepository
    {
        private readonly EvolutionApiContext _context;
        public RCourseBgListRepository(EvolutionApiContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<TCourse>> GetCoursePageAsync(int CompanyId)
        {
            return await _context.TCourses
                .Where(c => c.CompanyId == CompanyId&& c.IsDraft == false)
                .OrderBy(c => c.CourseId)                
                .ToListAsync();
        }
        public async Task<IEnumerable<TCourseHashTag>> GetCourseHashTagAsync(int CourseId)
        {
            return await _context.TCourseHashTags
                .Where(ch => ch.CourseId == CourseId)
                .ToListAsync();
        }
        public async Task<IEnumerable<TCourseHashTag>> GetCourseHashTagsAsync(List<int> courseIds)
        {
            return await _context.TCourseHashTags
                .Where(ch => courseIds.Contains(ch.CourseId))
                .ToListAsync();
        }
    }
}
