using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Repositories.Interfaces;
using prjEvolutionAPI.Responses;
using System.Linq;                  // EF Core / LINQ
using System.Linq.Dynamic.Core;     // ← 這行千萬不能少

namespace prjEvolutionAPI.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly EvolutionApiContext _context;
        public CompanyRepository(Models.EvolutionApiContext context)
        {
            _context = context;
        }
        public void Add(TCompany company)
        {
            _context.TCompanies.Add(company);
        }
        public async Task<TCompany?> GetByIdAsync(int companyId)
        {
            return await _context.TCompanies.FindAsync(companyId);
        }
        public async Task<TCompany?> GetByUserIdAsync(int userId)
        {
            return await _context.TUsers
                .Where(u => u.UserId == userId)
                .Select(u => u.Company)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        public async Task<TCompany?> GetByEmailAsync(string email)
        {
            return _context.TCompanies.FirstOrDefault(c => c.CompanyEmail == email);
        }

        public async Task<int> GetCompanyCountAsync()
        {
            return await _context.TCompanies
                         .AsNoTracking()
                         .CountAsync(c => c.IsActive);
        }

        public async Task<Responses.PagedResult<CompanyListDTO>> GetPagedAsync(
        int start,
        int limit,
        string sortField,
        int sortOrder,
        IDictionary<string, string> filters)
        {
            // 1. 建立 IQueryable
            var query = _context.TCompanies.AsNoTracking().AsQueryable();

            // 2. 依 filters 動態加入 Where
            foreach (var kv in filters)
            {
                var propName = kv.Key;    // e.g. "companyName" 或 "isActive"
                var raw = kv.Value;       // e.g. "Foo" 或 "true"
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                switch (propName)
                {
                    case "companyName":
                        query = query.Where(c => c.CompanyName.Contains(raw));
                        break;
                    case "isActive":
                        if (bool.TryParse(raw, out var b))
                            query = query.Where(c => c.IsActive == b);
                        break;
                    case "companyEmail":
                        query = query.Where(c => c.CompanyEmail.Contains(raw));
                        break;
                    case "createdAt":
                        if (DateTime.TryParse(raw, out var filterDate))
                        {
                            query = query.Where(c =>
                                c.CreatedAt.Date == filterDate.Date);
                        }
                        break;
                    // 如果還有其他欄位要篩選，就在這裡加 case
                    default:
                        break;
                }
            }

            // 3. 先拿總筆數
            var total = await query.CountAsync();

            // 4. 動態排序（需安裝 System.Linq.Dynamic.Core）
            if (!string.IsNullOrEmpty(sortField))
            {
                var dir = sortOrder == 1 ? "asc" : "desc";
                var config = new ParsingConfig
                {
                    IsCaseSensitive = false
                };
                query = query.OrderBy(config, $"{sortField} {dir}");
            }
            else
            {
                // 預設排序
                query = query.OrderByDescending(c => c.CreatedAt);
            }

            // 5. Skip / Take 分頁
            var items = await query
                .Skip(start)
                .Take(limit)
                .Select(c => new CompanyListDTO
                {
                    CompanyId = c.CompanyId,
                    CompanyName = c.CompanyName,
                    CompanyEmail = c.CompanyEmail,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return new Responses.PagedResult<CompanyListDTO>
            {
                Data = items,
                Total = total
            };
        }
    }
}
