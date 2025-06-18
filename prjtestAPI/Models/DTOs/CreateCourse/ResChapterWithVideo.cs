namespace prjEvolutionAPI.Models.DTOs.CreateCourse
{
    public class ResChapterWithVideo
    {
        public string ChapterTitle { get; set; } = null!; // ChapterTitle
        public string ChapterDes { get; set; } = null!; // ChapterDes
        public List<ResFinalVideo> videos { get; set; } = new List<ResFinalVideo>(); // videos
    }
}
