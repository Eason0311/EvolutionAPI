namespace prjEvolutionAPI.Models.DTOs.User
{
    public class EmpOrderDTO
    {
        public int CourseId { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }

        public string CourseTitle { get; set; } = null!;

        public string? CourseDes { get; set; }

        public string CoverImagePath { get; set; } = null!;
    }
}
