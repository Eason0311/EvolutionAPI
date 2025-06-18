using Azure;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.CompanyManage;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Responses;
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
        Task<int> GetUserCountAsync();
        Task<PagedResult<EmployeesListDTO>> GetPagedAsync(
       int start,
       int limit,
       string sortField,
       int sortOrder,
       IDictionary<string, string> filters,
       int companyId);
        Task<int> GetCompanyIdAsync(int userId);
        Task<IEnumerable<TUser>> GetUsersByDepartmentsAsync(int[] depIds,int userId);
        Task<IEnumerable<TUser>> GetEmployeesByCompanyIdAsync(int companyId);
    }
}