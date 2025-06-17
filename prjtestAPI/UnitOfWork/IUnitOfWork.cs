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
    IHashTagListRepository HashTagLists { get; }
    ICourseAccessRepository CourseAccesses { get; }
    ICourseBgListRepository CourseBgList { get; }
    Task<int> CompleteAsync();
    Task ExecuteTransactionAsync(Func<Task> action);
}
