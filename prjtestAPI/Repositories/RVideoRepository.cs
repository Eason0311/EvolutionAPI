using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class RVideoRepository : IVideoRepository
    {
        private readonly EvolutionApiContext _context;

        public RVideoRepository(EvolutionApiContext context)
        {
            _context = context;
        }

        public async Task<TVideo> GetByIdAsync(int id)
        {
            return await _context.TVideos.FindAsync(id);
        }

        public async Task<IEnumerable<TVideo>> GetAllAsync()
        {
            return await _context.TVideos.ToListAsync();
        }

        public async Task<TVideo> AddAsync(TVideo video)
        {
            var result = await _context.TVideos.AddAsync(video);
            return result.Entity;
        }

        public void Update(TVideo video)
        {
            _context.TVideos.Update(video);
        }

        public void Delete(TVideo video)
        {
            _context.TVideos.Remove(video);
        }
    }
}
