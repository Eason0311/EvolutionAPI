using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface IEmpOrderRepository
    {
        // --- Create ---
        /// <summary>
        /// 新增一筆「員工訂單」到 DbContext，實際寫入由 UnitOfWork.SaveChangesAsync() 處理
        /// </summary>
        //Task AddAsync(TEmpOrder entity);

        // --- Read ---
        /// <summary>
        /// 根據訂單主鍵 (OrderId) 取得單筆員工訂單
        /// </summary>
        //Task<TEmpOrder> GetByIdAsync(int orderId);

        /// <summary>
        /// 取得某個員工 (EmployeeId) 名下的所有訂單，用於顯示個人訂單歷史
        /// </summary>
        Task<IEnumerable<TEmpOrder>> GetByEmployeeIdAsync(int employeeId);

        /// <summary>
        /// 取得所有員工訂單（必要時可換成分頁查詢）
        /// </summary>
        //Task<IEnumerable<TEmpOrder>> GetAllAsync();

        // --- Update ---
        /// <summary>
        /// 標記某筆 EmployeeOrder 為已修改，實際更新由 UnitOfWork.SaveChangesAsync() 執行
        /// </summary>
        //void Update(TEmpOrder entity);

        // --- Delete ---
        /// <summary>
        /// 標示某筆 EmployeeOrder 將被刪除，實際刪除由 UnitOfWork.SaveChangesAsync() 執行
        /// </summary>
        //void Remove(TEmpOrder entity);

        // --- Exists / 其他查詢 ---
        /// <summary>
        /// 檢查某筆訂單是否存在（只回傳 true/false，不載入整筆 Entity）
        /// </summary>
        //Task<bool> ExistsAsync(int orderId);

        /// <summary>
        /// （可選）根據訂單狀態查詢，例如：已付款、待付款、已取消等等
        /// </summary>
        //Task<IEnumerable<TEmpOrder>> GetByStatusAsync(int employeeId, OrderStatus status);
    }
}
