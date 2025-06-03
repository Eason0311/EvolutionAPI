using prjtestAPI.Models;
using System.Threading.Tasks;

namespace prjtestAPI
{
    public interface IRefreshTokenRepository
    {
        Task<TRefreshToken?> GetByTokenAsync(string token);
        Task AddAsync(TRefreshToken refreshToken);
        void Update(TRefreshToken refreshToken);
        void Delete(TRefreshToken refreshToken);

    }
}