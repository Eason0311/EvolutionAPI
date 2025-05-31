using prjtestAPI;
using prjtestAPI.Repositories.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    Task<int> CompleteAsync();
}
