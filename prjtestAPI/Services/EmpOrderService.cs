using prjEvolutionAPI.Models.DTOs.User;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class EmpOrderService: IEmpOrderService
    {
        private readonly IUnitOfWork _uow;
        public EmpOrderService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IEnumerable<EmpOrderDTO>> GetEmpOrderListByIdAsync(int userId)
        {
            var chechUserId = await _uow.Users.GetByIdAsync(userId);
            if (chechUserId == null)
                throw new KeyNotFoundException($"找不到 User (UserID = {userId})");

            var orders = await _uow.EmpOrder.GetByEmployeeIdAsync(userId);
            if (orders == null || !orders.Any())
                return new List<EmpOrderDTO>();

            var dtoList = orders
                .Select(o => new EmpOrderDTO
                {
                    CourseId = o.CourseId,
                    Amount = o.Amount,
                    OrderDate = o.OrderDate,
                }).ToList();

            return dtoList;
        }
    }
}
