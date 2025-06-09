using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class PublisherRepository : IPublisherRepository
    {
        private readonly EvolutionApiContext _context;
        public PublisherRepository(EvolutionApiContext context) 
        {
            _context = context;
        }

        public async Task<List<CompanyListDTO>> GetAllAsync()
        {
            return await _context.TCompanies
                .AsNoTracking()
                .Select(c => new CompanyListDTO
                {
                    CompanyId = c.CompanyId,
                    CompanyName = c.CompanyName,
                    CompanyEmail = c.CompanyEmail,
                    CreatedAt = DateTime.Now,
                    IsActive = c.IsActive,
                }).ToListAsync();
        }
    }
}
