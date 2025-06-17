using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.LinePay;
using prjEvolutionAPI.Repositories;
using prjEvolutionAPI.Repositories.Interfaces;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<TEmpOrder> _empOrderRepo;

        public OrderService(IUnitOfWork uow,IRepository<TEmpOrder> empOrderRepo)
        {
            _uow = uow;
            _empOrderRepo = empOrderRepo;
        }

        public async Task<TCompOrder?> GetCompOrderByIdAsync(int compOrderId)
        {
            // 泛型 Repo 的 GetByIdAsync 會對應到 DbSet.FindAsync(pk)
            return await _uow.CompOrders.GetByIdAsync(compOrderId);
        }

        public async Task<TEmpOrder?> GetEmpOrderByIdAsync(int empOrderId)
        {
            return await _uow.EmpOrders.GetByIdAsync(empOrderId);
        }

        public async Task<bool> UpdateCompOrderPaidAsync(int compOrderId, bool isPaid)
        {
            var order = await GetCompOrderByIdAsync(compOrderId);
            if (order == null) return false;

            order.IsPaid = isPaid;
            _uow.CompOrders.Update(order);
            await _uow.CompleteAsync();
            return true;
        }

        public async Task<bool> UpdateEmpOrderPaidAsync(int empOrderId, bool isPaid)
        {
            var order = await GetEmpOrderByIdAsync(empOrderId);
            if (order == null) return false;

            order.IsPaid = isPaid;
            _uow.EmpOrders.Update(order);
            await _uow.CompleteAsync();
            return true;
        }

        public async Task<int[]> CreateCompOrdersAsync(IEnumerable<CreateCompOrderDto> dtos)
        {
            var entities = new List<TCompOrder>();

            foreach (var dto in dtos)
            {
                var course = await _uow.Course.GetByIdAsync(dto.CourseId)
                             ?? throw new KeyNotFoundException($"找不到課程 {dto.CourseId}");

                entities.Add(new TCompOrder
                {
                    BuyerCompanyId = dto.CompanyId,
                    CourseId = dto.CourseId,
                    OrderDate = DateTime.UtcNow,
                    Amount = course.Price,
                    IsPaid = false
                });
            }

            _uow.CompOrders.AddRange(entities);
            await _uow.CompleteAsync();

            return entities.Select(e => e.OrderId).ToArray();
        }

        public async Task<int[]> CreateEmpOrdersAsync(IEnumerable<CreateEmpOrderDto> dtos)
        {
            var entities = new List<TEmpOrder>();

            foreach (var dto in dtos)
            {
                var course = await _uow.Course.GetByIdAsync(dto.CourseId)
                             ?? throw new KeyNotFoundException($"找不到課程 {dto.CourseId}");

                entities.Add(new TEmpOrder
                {
                    BuyerUserId = dto.UserId,
                    CourseId = dto.CourseId,
                    OrderDate = DateTime.UtcNow,
                    Amount = course.Price,
                    IsPaid = false
                });
            }

            _uow.EmpOrders.AddRange(entities);
            await _uow.CompleteAsync();

            return entities.Select(e => e.OrderId).ToArray();
        }

        public async Task MarkOrderPaidAsync(int orderId, bool isCompany)
        {
            if (isCompany)
                await UpdateCompOrderPaidAsync(orderId, true);
            else
                await UpdateEmpOrderPaidAsync(orderId, true);
        }

        public async Task<int> CreateCompOrderAsync(TCompOrder order)
        {
            await _uow.Repository<TCompOrder>().AddAsync(order);
            await _uow.CompleteAsync();
            return order.OrderId;
        }

        public async Task<int> CreateEmpOrderAsync(TEmpOrder order)
        {
            await _uow.Repository<TEmpOrder>().AddAsync(order);
            await _uow.CompleteAsync();
            return order.OrderId;
        }

        public async Task MarkOrdersPaidByPaymentIdAsync(int paymentId)
        {
            var details = await _uow.PaymentDetails.GetWhereAsync(d => d.PaymentId == paymentId);

            foreach (var detail in details)
            {
                if (detail.CompOrderId.HasValue)
                {
                    await MarkOrderPaidAsync(detail.CompOrderId.Value, isCompany: true);
                }
                else if (detail.EmpOrderId.HasValue)
                {
                    await MarkOrderPaidAsync(detail.EmpOrderId.Value, isCompany: false);
                }
            }
        }

        public async Task<int[]> GetUserOwnCourse(int userId)
        {
            var orders = await _uow.Repository<TEmpOrder>().GetWhereAsync(u => u.BuyerUserId == userId);

            var courseIds = orders.Select(c => c.CourseId).ToArray();

            return courseIds;
        }

        public async Task<IEnumerable<(string ProductName, int Quantity, decimal UnitPrice, decimal TotalPrice)>>
        GetOrderDetailsAsync(int paymentId)
        {
            var result = new List<(string, int, decimal, decimal)>();

            // 1. 先撈出所有明細，再過濾
            var allDetails = await _uow
                .Repository<TPaymentDetail>()
                .GetAllAsync();  // <-- 假設有這個

            var paymentDetails = allDetails
                .Where(pd => pd.PaymentId == paymentId)
                .ToList();

            // 2. 依照 CompOrderId / EmpOrderId 拿訂單與課程
            foreach (var pd in paymentDetails)
            {
                if (pd.CompOrderId.HasValue)
                {
                    var comp = await _uow.Repository<TCompOrder>()
                        .GetByIdAsync(pd.CompOrderId.Value);
                    var course = await _uow.Repository<TCourse>()
                        .GetByIdAsync(comp.CourseId);

                    decimal unit = comp.Amount.GetValueOrDefault(); ;
                    int quantity = 1;
                    result.Add((course.CourseTitle, quantity, unit, unit * quantity));
                }
                else if (pd.EmpOrderId.HasValue)
                {
                    var emp = await _uow.Repository<TEmpOrder>()
                        .GetByIdAsync(pd.EmpOrderId.Value);
                    var course = await _uow.Repository<TCourse>()
                        .GetByIdAsync(emp.CourseId);

                    decimal unit = emp.Amount.GetValueOrDefault(); ;
                    int quantity = 1;
                    result.Add((course.CourseTitle, quantity, unit, unit * quantity));
                }
            }

            return result;
        }
    }
}
