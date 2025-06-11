namespace prjEvolutionAPI.Models.DTOs.CreateCourse
{
    public class VVideoDTO
    {
        public int? ChapterId { get; set; } // ChapterId
        public string Title { get; set; } // VideoTitle
        public IFormFile? VideoFile { get; set; } // VideoFile
        public string ConnectionId { get; set; }  // ← 新增
        public string clientRequestId { get; set; } = null!; // 用於追蹤請求的唯一識別碼
    }
}
