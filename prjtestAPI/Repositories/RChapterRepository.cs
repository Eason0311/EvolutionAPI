using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class RChapterRepository : IChapterRepository
    {
        private readonly EvolutionApiContext _context;

        public RChapterRepository(EvolutionApiContext context)
        {
            _context = context;
        }

        public async Task<TCourseChapter> GetByIdAsync(int id)
        {
            return await _context.TCourseChapters.FindAsync(id);
        }

        public async Task<IEnumerable<TCourseChapter>> GetAllAsync()
        {
            return await _context.TCourseChapters.ToListAsync();
        }

        public async Task<TCourseChapter> AddAsync(TCourseChapter chapter)
        {
            var result = await _context.TCourseChapters.AddAsync(chapter);
            return result.Entity;
        }

        public void Update(TCourseChapter chapter)
        {
            _context.TCourseChapters.Update(chapter);
        }

        public void Delete(TCourseChapter chapter)
        {
            _context.TCourseChapters.Remove(chapter);
        }
    }
}
