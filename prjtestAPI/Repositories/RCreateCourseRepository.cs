using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class RCreateCourseRepository : ICreateCourseRepository
    {
        private readonly EvolutionApiContext _context;

        public RCreateCourseRepository(EvolutionApiContext context)
        {
            _context = context;
        }

        public async Task<TCourse> GetByIdAsync(int id)
        {
            return await _context.TCourses.FindAsync(id);
        }

        public async Task<IEnumerable<TCourse>> GetAllAsync()
        {
            return await _context.TCourses.ToListAsync();
        }

        public async Task<TCourse> AddAsync(TCourse course)
        {
            var entry = await _context.TCourses.AddAsync(course);
            return entry.Entity; // ✅ 回傳資料本體
        }

        public void Update(TCourse course)
        {
            _context.TCourses.Update(course);
        }
        public void Delete(TCourse course)
        {
            _context.TCourses.Remove(course);
        }
        public async Task<List<TCourseChapter>> GetChaptersWithVideosByCourseIdAsync(int courseId)
        {
            return await _context.TCourseChapters
                .Where(c => c.CourseId == courseId)
                .Include(c => c.TVideos)
                .OrderBy(c => c.ChapterId)
                .ToListAsync();
        }
    }
}
