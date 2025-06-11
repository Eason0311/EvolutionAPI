using Azure;
using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.CompanyManage;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjtestAPI;
using prjtestAPI.Models;
using prjtestAPI.Repositories.Interfaces;
using System;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using prjEvolutionAPI.Responses;

public class UserRepository : IUserRepository
{
    private readonly EvolutionApiContext _context;
    public UserRepository(EvolutionApiContext context)
    {
        _context = context;
    }

    public async Task<TUser?> GetByEmailAsync(string email)
    {
        return await _context.TUsers.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<TUser?> GetByIdAsync(int userId)
    {
        return await _context.TUsers.FindAsync(userId);
    }

    public void Add(TUser user)
    {
        _context.TUsers.Add(user);
        // 不在這裡呼叫 SaveChangesAsync()
    }

    public void Update(TUser user)
    {
        _context.TUsers.Update(user);
        // 同樣不在這裡呼叫 SaveChangesAsync()
    }
    public async Task<TUser?> GetByIdWithDepAsync(int userId)
    {
        return await _context.TUsers
            .Include(u => u.Company)
           .Include(u => u.UserDepNavigation)
           .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<int> GetUserCountAsync()
    {
        return await _context.TUsers
        .AsNoTracking()
                         .CountAsync(u => u.IsEmailConfirmed);
    }

    public async Task<prjEvolutionAPI.Responses.PagedResult<EmployeesListDTO>> GetPagedAsync(
        int start,
        int limit,
        string sortField,
        int sortOrder,
        IDictionary<string, string> filters, int companyId)
    {
        // 1. 建立 IQueryable
        var query = _context.TUsers.AsNoTracking().Where(c => c.CompanyId == companyId).AsQueryable();

        // 2. 依 filters 動態加入 Where
        foreach (var kv in filters)
        {
            var propName = kv.Key;    // e.g. "companyName" 或 "isActive"
            var raw = kv.Value;       // e.g. "Foo" 或 "true"
            if (string.IsNullOrWhiteSpace(raw))
                continue;

            switch (propName)
            {
                case "userName":
                    query = query.Where(c => c.Username.Contains(raw));
                    break;
                case "userStatus":
                    query = query.Where(c => c.UserStatus.Contains(raw));
                    break;
                case "email":
                    query = query.Where(c => c.Email.Contains(raw));
                    break;
                case "userDep":
                    query = query.Where(c => c.UserDepNavigation.DepName.Contains(raw));
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
            query = query.OrderByDescending(c => c.UserId);
        }

        // 5. Skip / Take 分頁
        var items = await query
            .Skip(start)
            .Take(limit)
            .Select(c => new EmployeesListDTO
            {
                UserId = c.UserId,
                Username = c.Username,
                Email = c.Email,
                UserStatus = c.UserStatus,
                UserDep = c.UserDepNavigation.DepName,
            })
            .ToListAsync();

        return new prjEvolutionAPI.Responses.PagedResult<EmployeesListDTO>
        {
            Data = items,
            Total = total
        };
    }

    public async Task<int> GetCompanyIdAsync(int userId)
    {
            return await _context.TUsers.Where(u => u.UserId == userId).Select(u => u.CompanyId).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TUser>> GetUsersByDepartmentsAsync(int[] depIds)
    {
        return await _context.TUsers
            .Where(u => depIds.Contains(u.UserDep))
            .ToListAsync();
    }
}