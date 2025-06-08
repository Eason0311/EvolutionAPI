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
    private readonly ICompOrderRepository _compOrder;
    private readonly IEmpOrderRepository _empOrder;
    private readonly ICourseRepository _course;
    private readonly IQuizResultsRepository _quizResults;
    private readonly IHashTagListRepository _hashTagList;

    public UnitOfWork(
        EvolutionApiContext context,
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        ICompanyRepository company,
        IDepListRepository depList,
        IEmpOrderRepository empOrder,
        ICompOrderRepository compOrder,
        ICourseRepository course,
        IQuizResultsRepository quizResults,
        IHashTagListRepository hashTagList
        )
    {
        _context = context;
        _users = users;
        _refreshTokens = refreshTokens;
        _company = company;
        _depList = depList;
        _compOrder = compOrder;
        _empOrder = empOrder;
        _course = course;
        _quizResults = quizResults;
        _hashTagList = hashTagList;
    }

    public IUserRepository Users => _users;
    public IRefreshTokenRepository RefreshTokens => _refreshTokens;
    public ICompanyRepository Company => _company;
    public IDepListRepository DepList => _depList;
    public IEmpOrderRepository EmpOrder => _empOrder;
    public ICompOrderRepository CompOrder => _compOrder;
    public ICourseRepository Course => _course;
    public IQuizResultsRepository QuizResults => _quizResults;
    public IHashTagListRepository HashTagList => _hashTagList;

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
