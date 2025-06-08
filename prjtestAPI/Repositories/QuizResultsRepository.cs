using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class QuizResultsRepository : IQuizResultsRepository
    {
        private readonly EvolutionApiContext _context;
        public QuizResultsRepository(EvolutionApiContext context)
        {
            _context = context;
        }
        public async Task<int> GetQuizResultsCountAsync()
        {
            return await _context.TQuizResults.AsNoTracking().CountAsync();
        }
    }
}
