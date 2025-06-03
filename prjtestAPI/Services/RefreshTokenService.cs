using prjEvolutionAPI.Services.Interfaces;
using prjtestAPI.Models;
using prjtestAPI;

namespace prjEvolutionAPI.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _rtRepo;
        private readonly IUnitOfWork _uow;

        public RefreshTokenService(
            IRefreshTokenRepository rtRepo,
            IUnitOfWork uow)
        {
            _rtRepo = rtRepo;
            _uow = uow;
        }

        public async Task<TRefreshToken?> GetByTokenAsync(string token)
        {
            return await _rtRepo.GetByTokenAsync(token);
        }

        public async Task<bool> RevokeAsync(string token)
        {
            // 1. 先找出這個 refresh token
            TRefreshToken? rt = await _rtRepo.GetByTokenAsync(token);
            if (rt == null)
                return false;

            // 2. 標示此實體為「已撤銷」
            rt.IsRevoked = true;
            _rtRepo.Update(rt);   // 只標示 update，尚未 commit

            // 3. 最後一次性 commit
            await _uow.CompleteAsync();
            return true;
        }
    }
}
