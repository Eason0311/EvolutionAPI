using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface ICompOrderRepository
    {
        // 1. Create：新增一筆公司訂單
        //Task AddAsync(TCompOrder entity);

        // 2. Read：
        // 2.1 依據訂單 Id 取得單筆訂單
        //Task<TCompOrder> GetByIdAsync(int orderId);

        // 2.2 取得該公司所有訂單（可拿來顯示清單）
        //Task<IEnumerable<TCompOrder>> GetByCompanyIdAsync(int companyId);

        // 2.3 （必要時）取得所有公司訂單（若不需要一次查全部，也可改用分頁或條件查詢）
        //Task<IEnumerable<TCompOrder>> GetAllAsync();

        // 3. Update：更新某筆公司訂單（只更新已載入的 Entity 即可，真正儲存由 UnitOfWork 或 DbContext.SaveChanges 進行）
        //void Update(TCompOrder entity);

        // 4. Delete：刪除某筆公司訂單
        //void Remove(TCompOrder entity);

        // 5.（可選）檢查某筆訂單是否存在
        //Task<bool> ExistsAsync(int orderId);
    }
}
