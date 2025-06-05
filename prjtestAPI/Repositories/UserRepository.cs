using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjtestAPI;
using prjtestAPI.Models;
using prjtestAPI.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

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
           .Include(u => u.UserDepNavigation)
           .FirstOrDefaultAsync(u => u.UserId == userId);
    }
}
