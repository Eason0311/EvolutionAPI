using prjtestAPI;
using prjtestAPI.Repositories.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    Task<int> CompleteAsync();
    Task ExecuteTransactionAsync(Func<Task> action);
}
