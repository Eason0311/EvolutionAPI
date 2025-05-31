using prjtestAPI.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace prjtestAPI.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(TUser user);
        Task<TRefreshToken> GenerateRefreshToken(int userId, string? ipAddress, string? userAgent);
        ClaimsPrincipal? ValidateAccessToken(string token);
        int RefreshTokenExpirationDays { get; }
    }
}
