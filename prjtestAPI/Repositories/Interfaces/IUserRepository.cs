using prjtestAPI.Models;
using System.Threading.Tasks;

namespace prjtestAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<TUser> GetByEmailAsync(string email);
        Task<TUser> GetByIdAsync(int userId);
        Task AddAsync(TUser user);
        Task UpdateAsync(TUser user);
    }
}