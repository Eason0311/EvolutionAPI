using prjEvolutionAPI.Models;
using prjEvolutionAPI.Repositories.Interfaces;
using prjEvolutionAPI.Services.Interfaces;
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
    ICourseRepository Course { get; }
    IQuizResultsRepository QuizResults { get; }
    IHashTagListRepository HashTagList { get; }
    IPublisherRepository Publisher { get; }
    ICreateCourseRepository CreateCourse { get; }
    IChapterRepository Chapters { get; }
    IVideoRepository Videos { get; }
    ICourseHashTagRepository CourseHashTags { get; }
    ICourseAccessRepository CourseAccesses { get; }
    IRepository<TCompOrder> CompOrders { get; }
    IRepository<TEmpOrder> EmpOrders { get; }
    IPaymentRepository Payments { get; }
    IRepository<TPaymentDetail> PaymentDetails { get; }
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    Task<int> CompleteAsync();
    Task ExecuteTransactionAsync(Func<Task> action);
}
