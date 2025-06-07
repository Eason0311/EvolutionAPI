namespace prjEvolutionAPI.Models.DTOs.Course
{
    public class CourseDTO
    {
        public int CourseId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string CourseTitle { get; set; } = null!;
        public string? CourseDes { get; set; }
        public bool IsPublic { get; set; }
        public string CoverImagePath { get; set; } = null!;
        public int Price { get; set; }
    }
}
