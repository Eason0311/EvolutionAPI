using prjtestAPI;
using prjtestAPI.Repositories.Interfaces;
using prjEvolutionAPI.Services.Interfaces;
using prjEvolutionAPI.Repositories.Interfaces;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EvolutionApiContext _context;
    private readonly Dictionary<Type, object> _repos = new();

    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly ICompanyRepository _company;
    private readonly IDepListRepository _depList;
    private readonly ICompOrderRepository _compOrder;
    private readonly IEmpOrderRepository _empOrder;
    private readonly ICourseRepository _course;
    private readonly IQuizResultsRepository _quizResults;
    private readonly IHashTagListRepository _hashTagList;
    private readonly IPublisherRepository _publisher;
    private readonly ICreateCourseRepository _createCourse;
    private readonly IChapterRepository _chapter;
    private readonly IVideoRepository _video;
    private readonly ICourseHashTagRepository _courseHashTag;
    private readonly ICourseAccessRepository _courseAccess;
    public IRepository<TCompOrder> CompOrders { get; }
    public IRepository<TEmpOrder> EmpOrders { get; }
    public IPaymentRepository Payments { get; }
    public IRepository<TPaymentDetail> PaymentDetails { get; }
    private readonly ICourseBgListRepository _courseBgList;

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
        IHashTagListRepository hashTagList,
        IPublisherRepository publisher,
        ICreateCourseRepository createCourse,
        IChapterRepository chapter,
        IVideoRepository video,
        ICourseHashTagRepository courseHashTag,
        ICourseAccessRepository courseAccess,
        IRepository<TCompOrder> compOrders,
        IRepository<TEmpOrder> empOrders,
        IPaymentRepository payments,
        IRepository<TPaymentDetail> paymentDetails,
        ICourseBgListRepository courseBgList
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
        _publisher = publisher;
        _video = video;
        _courseHashTag = courseHashTag;
        _courseAccess = courseAccess;
        _createCourse = createCourse;
        _chapter = chapter;
        CompOrders = compOrders;
        EmpOrders = empOrders;
        Payments = payments;
        PaymentDetails = paymentDetails;
        _courseBgList = courseBgList;
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
    public IPublisherRepository Publisher => _publisher;
    public ICreateCourseRepository CreateCourse => _createCourse;
    public IChapterRepository Chapters => _chapter;
    public IVideoRepository Videos => _video;
    public ICourseHashTagRepository CourseHashTags => _courseHashTag;
    public IHashTagListRepository HashTagLists => _hashTagList;
    public ICourseAccessRepository CourseAccesses  => _courseAccess;

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);
        if (!_repos.ContainsKey(type))
            _repos[type] = new Repository<TEntity>(_context);
        return (IRepository<TEntity>)_repos[type];
    }
    public ICourseBgListRepository CourseBgList => _courseBgList;
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
