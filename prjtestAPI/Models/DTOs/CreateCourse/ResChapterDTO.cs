namespace prjEvolutionAPI.Models.DTOs.CreateCourse
{
    public class ResChapterDTO
    {
        public int ChapterID { get; set; } // ChapterID
        public int CourseId { get; set; } // CourseID
        public string ChapterTitle { get; set; } = null!; // ChapterTitle
        public string ChapterDes { get; set; } = null!; // ChapterDes
    }
}
