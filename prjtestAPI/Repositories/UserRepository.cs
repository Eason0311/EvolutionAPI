using Microsoft.EntityFrameworkCore;
using prjtestAPI;
using prjtestAPI.Data;
using prjtestAPI.Models;
using prjtestAPI.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

public class UserRepository : IUserRepository
{
    private readonly TestApiContext _context;
    public UserRepository(TestApiContext context)
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
}
