using prjtestAPI.Models;

namespace prjtestAPI.Services.Interfaces
{
    public interface IUserActionTokenService
    {
        Task<TUserActionToken?> GetValidTokenAsync(string token, string tokenType);
        Task<TUserActionToken> CreateTokenAsync(int userId, string tokenType, TimeSpan validFor);
        Task MarkTokenAsUsedAsync(TUserActionToken token);
    }
}
