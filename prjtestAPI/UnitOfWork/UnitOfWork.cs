using prjtestAPI;
using prjtestAPI.Repositories.Interfaces;
using prjEvolutionAPI.Services.Interfaces;
using prjEvolutionAPI.Repositories.Interfaces;
using prjEvolutionAPI.Models;

public class UnitOfWork : IUnitOfWork
{
    private readonly EvolutionApiContext _context;
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly ICompanyRepository _company;
    private readonly IDepListRepository _depList;

    public UnitOfWork(
        EvolutionApiContext context,
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        ICompanyRepository company,
        IDepListRepository depList
        )
    {
        _context = context;
        _users = users;
        _refreshTokens = refreshTokens;
        _company = company;
        _depList = depList;
    }

    public IUserRepository Users => _users;
    public IRefreshTokenRepository RefreshTokens => _refreshTokens;
    public ICompanyRepository Company => _company;
    public IDepListRepository DepList => _depList;

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
