using prjEvolutionAPI.Models.DTOs.User;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface IEmpOrderService
    {
        Task<IEnumerable<EmpOrderDTO>> GetEmpOrderListByIdAsync(int userId);
    }
}
