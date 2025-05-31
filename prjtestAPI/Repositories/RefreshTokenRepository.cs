using Microsoft.EntityFrameworkCore;
using prjtestAPI;
using prjtestAPI.Data;
using prjtestAPI.Models;
using System;
using System.Threading.Tasks;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly TestApiContext _context;
    public RefreshTokenRepository(TestApiContext context)
    {
        _context = context;
    }

    public async Task<TRefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.TRefreshTokens
            .Include(rt => rt.User)  // 如需同時載入 User 資料
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task AddAsync(TRefreshToken refreshToken)
    {
        await _context.TRefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TRefreshToken refreshToken)
    {
        _context.TRefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(TRefreshToken entity)
    {
        _context.TRefreshTokens.Remove(entity);
        await _context.SaveChangesAsync();
    }
    public void Update(TRefreshToken token)
    {
        _context.TRefreshTokens.Update(token);
    }
}
