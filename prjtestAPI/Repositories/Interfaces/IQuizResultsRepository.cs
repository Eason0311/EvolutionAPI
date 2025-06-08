namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface IQuizResultsRepository
    {
        Task<int> GetQuizResultsCountAsync();
    }
}
