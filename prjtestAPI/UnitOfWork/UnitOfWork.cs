using prjtestAPI.Data;
using prjtestAPI;
using prjtestAPI.Repositories.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private readonly TestApiContext _context;
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;

    public UnitOfWork(
        TestApiContext context,
        IUserRepository users,
        IRefreshTokenRepository refreshTokens)
    {
        _context = context;
        _users = users;
        _refreshTokens = refreshTokens;
    }

    public IUserRepository Users => _users;
    public IRefreshTokenRepository RefreshTokens => _refreshTokens;

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task ExecuteTransactionAsync(Func<Task> action)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            await action();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
