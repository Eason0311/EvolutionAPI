namespace prjEvolutionAPI.Services.Interfaces
{
    public interface IOpenAIService
    {
        Task<string> TranscribeAudioAsync(int videoId);
        Task<string> GenerateSummaryAsync(string transcript);
        
    }
}
