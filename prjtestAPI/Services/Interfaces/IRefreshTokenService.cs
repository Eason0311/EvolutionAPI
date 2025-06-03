using prjtestAPI.Models;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<TRefreshToken?> GetByTokenAsync(string token);
        Task<bool> RevokeAsync(string token);
    }
}
