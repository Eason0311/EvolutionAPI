using prjtestAPI.Data;
using prjtestAPI;
using prjtestAPI.Repositories.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private readonly TestApiContext _context;
    private IUserRepository? _users;
    private IRefreshTokenRepository? _refreshTokens;

    public UnitOfWork(TestApiContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
