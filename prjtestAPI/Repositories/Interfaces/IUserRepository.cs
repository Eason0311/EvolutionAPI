using prjEvolutionAPI.Models;
using prjtestAPI.Models;
using System.Threading.Tasks;

namespace prjtestAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<TUser?> GetByEmailAsync(string email);
        Task<TUser?> GetByIdAsync(int userId);
        Task<TUser?> GetByIdWithDepAsync(int userId);
        void Add(TUser user);
        void Update(TUser user);
    }
}