namespace prjEvolutionAPI.Models.DTOs.CreateCourse
{
    public class ResCourseDTO
    {
        public int CourseId { get; set; }
        public int CompanyId { get; set; }
        public string CourseTitle { get; set; }

        public string CourseDes { get; set; }

        public int Price { get; set; }
        public bool IsPublic { get; set; }

        public string CoverImage { get; set; }
    }
}
