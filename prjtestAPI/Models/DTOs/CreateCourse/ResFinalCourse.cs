namespace prjEvolutionAPI.Models.DTOs.CreateCourse
{
    public class ResFinalCourse
    {
        public string CourseTitle { get; set; } = null!; // CourseTiTle
        public string CourseDes { get; set; } = null!; // CourseDes
        public string CoverImagePath { get; set; } = null!; // CoverImagePath
        public bool IsPublic { get; set; } // IsPublic
        public int Price { get; set; } // Price
        public List<ResChapterWithVideo> chapterWithVideos { get; set; } = new List<ResChapterWithVideo>(); // chapterWithVideos 
    }
}
