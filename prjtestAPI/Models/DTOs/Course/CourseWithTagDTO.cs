namespace prjEvolutionAPI.Models.DTOs.Course
{
    public class CourseWithTagDTO
    {
        public List<int> TagIds { get; set; } = new();
        public int CourseId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string CourseTitle { get; set; } = null!;
        public string? CourseDes { get; set; }
        public bool IsPublic { get; set; }
        public string CoverImagePath { get; set; } = null!;
    }
}
