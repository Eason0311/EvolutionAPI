using prjtestAPI.Models;
using System.Threading.Tasks;

namespace prjtestAPI
{
    public interface IRefreshTokenRepository
    {
        Task<TRefreshToken> GetByTokenAsync(string token);
        Task AddAsync(TRefreshToken token);
        Task UpdateAsync(TRefreshToken token);
        Task DeleteAsync(TRefreshToken entity);
        void Update(TRefreshToken token);

    }
}