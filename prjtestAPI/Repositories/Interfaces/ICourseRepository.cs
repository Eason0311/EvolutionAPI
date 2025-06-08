using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Course;

namespace prjEvolutionAPI.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<TCourse>> GetAllAsync();

        // 依 ID 查單筆
        Task<CourseDTO?> GetByIdAsync(int courseId);
        Task<IEnumerable<CourseDTO>> GetByIdsAsync(IEnumerable<int> ids);

        // 依公司取得該公司的所有課程
        Task<IEnumerable<TCourse>> GetByCompanyIdAsync(int companyId);

        // 可加：關鍵字搜尋
        Task<List<CourseDTO>> SearchAsync(string query);

        // 可加：分頁查詢
        Task<(IEnumerable<CourseDTO> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize);

        // 檢查是否存在
        Task<bool> ExistsAsync(int courseId);

        // 新增、更新、刪除
        Task AddAsync(TCourse entity);
        void Update(TCourse entity);
        void Delete(TCourse entity);
        /// <summary>前綴提示：回傳最多 maxResults 個 CourseTitle</summary>
        Task<List<string>> GetTitleSuggestionsAsync(string prefix, int maxResults = 10);
        Task<int> GetCourseCountAsync();
        Task<List<CourseWithTagDTO>> GetRandomCoursesAsync(int count = 9);
    }
}
