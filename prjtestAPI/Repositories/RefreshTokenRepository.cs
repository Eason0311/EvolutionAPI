using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjtestAPI;
using prjtestAPI.Models;
using System;
using System.Threading.Tasks;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly EvolutionApiContext _context;
    public RefreshTokenRepository(EvolutionApiContext context)
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
        // 這裡只把資料「新增到 ChangeTracker」，但不 commit
        await _context.TRefreshTokens.AddAsync(refreshToken);
    }

    public void Update(TRefreshToken refreshToken)
    {
        // 標示此實體要更新到 DB，但不 commit
        _context.TRefreshTokens.Update(refreshToken);
    }

    public void Delete(TRefreshToken refreshToken)
    {
        // 標示此實體要刪除，但不 commit
        _context.TRefreshTokens.Remove(refreshToken);
    }
}
