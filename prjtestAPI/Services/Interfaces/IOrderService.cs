using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.LinePay;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<TCompOrder?> GetCompOrderByIdAsync(int compOrderId);
        Task<TEmpOrder?> GetEmpOrderByIdAsync(int empOrderId);
        Task<bool> UpdateCompOrderPaidAsync(int compOrderId, bool isPaid);
        Task<bool> UpdateEmpOrderPaidAsync(int empOrderId, bool isPaid);
        Task<int[]> CreateCompOrdersAsync(IEnumerable<CreateCompOrderDto> dtos);
        Task<int[]> CreateEmpOrdersAsync(IEnumerable<CreateEmpOrderDto> dtos);
        Task MarkOrderPaidAsync(int orderId, bool isCompany);
        Task<int> CreateCompOrderAsync(TCompOrder order);
        Task<int> CreateEmpOrderAsync(TEmpOrder order);
        Task MarkOrdersPaidByPaymentIdAsync(int paymentId);
        Task<int[]> GetUserOwnCourse(int userId);
    }
}