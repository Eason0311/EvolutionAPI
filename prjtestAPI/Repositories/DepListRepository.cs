using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class DepListRepository : IDepListRepository
    {
        private readonly EvolutionApiContext _context;
        public DepListRepository(EvolutionApiContext context)
        {
            _context = context;
        }
        public async Task<TDepList?> GetByIdAsync(int UserDe)
        {
            return await _context.TDepLists.FindAsync(UserDe);
        }
        public async Task<IEnumerable<TDepList?>> GetByCompanyIdAsync(int companyId)
        {
            return await _context.TDepLists
                .AsNoTracking()
                .Where(c => c.CompanyId == companyId )
                .ToArrayAsync();
        }
        public async Task<TDepList?> GetFirstOrDefaultAsync(string depName, int companyId)
        {
            return await _context.TDepLists
                         .FirstOrDefaultAsync(d =>
                             d.CompanyId == companyId &&
                             d.DepName == depName
                         );
        }
        public async Task AddAsync(TDepList depList)
        {
            await _context.TDepLists.AddAsync(depList);
        }

        public async Task<IEnumerable<ResDepListDTO>> GetAllDepartmentsAsync()
        {
            var departments = await _context.TDepLists
                .Select(n => new ResDepListDTO
                {
                    DepId = n.DepId,
                    DepName = n.DepName
                })
                .ToListAsync();
            return departments;
        }
    }
}
