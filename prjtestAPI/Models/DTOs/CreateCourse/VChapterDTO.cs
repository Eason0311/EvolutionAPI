namespace prjEvolutionAPI.Models.DTOs.CreateCourse
{
    public class VChapterDTO
    {
        public int CourseId { get; set; } // CourseId
        public string ChapterTitle { get; set; } = null!; // ChapterTitle
        public string ChapterDes { get; set; } = null!; // ChapterDes
        public string ConnectionId { get; set; }  // ← 新增
        public string clientRequestId { get; set; } = null!; // 用於追蹤請求的唯一識別碼
    }
}
