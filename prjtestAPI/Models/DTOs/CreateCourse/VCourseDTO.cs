namespace prjEvolutionAPI.Models.DTOs.CreateCourse
{
    public class VCourseDTO
    {
        public int? CompanyId { get; set; }

        public string CourseTitle { get; set; }

        public string CourseDes { get; set; }

        public int Price { get; set; }
        public bool IsPublic { get; set; }

        public IFormFile? CoverImage { get; set; }
        public string ConnectionId { get; set; }  // ← 新增
        public string clientRequestId { get; set; } = null!; // 用於追蹤請求的唯一識別碼
    }
}
