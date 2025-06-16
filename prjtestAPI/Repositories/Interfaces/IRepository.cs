using System.Linq.Expressions;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // 取得單筆（以主鍵）
        Task<T?> GetByIdAsync(int id);

        // 取得所有
        Task<IEnumerable<T>> GetAllAsync();

        // 以條件查詢
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // 新增
        Task AddAsync(T entity);

        // 更新
        void Update(T entity);

        // 刪除
        void Remove(T entity);
        void AddRange(IEnumerable<T> entity);
        Task<List<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);
    }
}
