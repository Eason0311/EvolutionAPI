using prjEvolutionAPI.Models.DTOs.CreateCourse;

namespace prjEvolutionAPI.Models.DTOs.CourseBgList
{
    public class ResCourseBgListDTO
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = null!;
        public bool IsPublic { get; set; }
        public List<int> CourseHashTags { get; set; } = new List<int>();
    }
}
