using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Responses;
using prjtestAPI.Models;
using System.Linq;                  // EF Core / LINQ
using System.Linq.Dynamic.Core;     // ← 這行千萬不能少

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        void Add(TCompany company);
        void Update(TCompany company);
        Task<TCompany?> GetByIdAsync(int companyId);
        Task<TCompany?> GetByUserIdAsync(int userId);
        Task<TCompany?> GetByEmailAsync(string email);
        Task<int> GetCompanyCountAsync();
        Task<Responses.PagedResult<CompanyListDTO>> GetPagedAsync(
       int start,
       int limit,
       string sortField,
       int sortOrder,
       IDictionary<string, string> filters);
    }
}
