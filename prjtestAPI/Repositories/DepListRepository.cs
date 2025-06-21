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

        public async Task<IEnumerable<ResDepListDTO>> GetAllDepartmentsAsync(int companyId)
        {
            var departments = await _context.TDepLists
                            .Where(n => n.CompanyId == companyId) // 篩選 CompanyID 為 2 的部門
                            .OrderBy(n => n.DepId)        // 確保有固定順序，Skip 才有意義
                            .Skip(1)                      // 忽略第一筆資料
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
