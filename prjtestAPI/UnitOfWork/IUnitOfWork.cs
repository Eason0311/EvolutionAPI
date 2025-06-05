using prjEvolutionAPI.Repositories.Interfaces;
using prjtestAPI;
using prjtestAPI.Repositories.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    ICompanyRepository Company { get; }
    IDepListRepository DepList { get; }
    IEmpOrderRepository EmpOrder { get; }
    ICompOrderRepository CompOrder { get; }
    Task<int> CompleteAsync();
    Task ExecuteTransactionAsync(Func<Task> action);
}
