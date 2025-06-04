using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjtestAPI.Models;
using prjtestAPI.Services.Interfaces;

namespace prjtestAPI.Services
{
    public class UserActionTokenService : IUserActionTokenService
    {
        private readonly TestApiContext _db;

        public UserActionTokenService(TestApiContext db)
        {
            _db = db;
        }

        public async Task<TUserActionToken?> GetValidTokenAsync(string token, string tokenType)
        {
            return await _db.TUserActionTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.Token == token &&
                    t.TokenType == tokenType &&
                    !t.IsUsed &&
                    t.ExpiryDate > DateTime.UtcNow);
        }

        public async Task<TUserActionToken> CreateTokenAsync(int userId, string tokenType, TimeSpan validFor)
        {
            var token = Guid.NewGuid().ToString("N");
            var tokenEntity = new TUserActionToken
            {
                UserId = userId,
                Token = token,
                TokenType = tokenType,
                ExpiryDate = DateTime.UtcNow.Add(validFor),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.TUserActionTokens.Add(tokenEntity);
            await _db.SaveChangesAsync();
            return tokenEntity;
        }

        public async Task MarkTokenAsUsedAsync(TUserActionToken token)
        {
            token.IsUsed = true;
            await _db.SaveChangesAsync();
        }
    }
}
