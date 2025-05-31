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

    public async Task AddAsync(TUser user)
    {
        await _context.TUsers.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TUser user)
    {
        _context.TUsers.Update(user);
        await _context.SaveChangesAsync();
    }
}
