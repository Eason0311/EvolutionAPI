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

        public OrderService(IUnitOfWork uow)
        {
            _uow = uow;
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
    }
}
