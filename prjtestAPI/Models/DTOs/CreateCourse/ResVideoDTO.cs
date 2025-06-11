namespace prjEvolutionAPI.Models.DTOs.CreateCourse
{
    public class ResVideoDTO
    {
        public int VideoID { get; set; } // VideoId
        public int ChapterId { get; set; } // ChapterId
        public string Title { get; set; } = null!; // VideoTitle
        public string VideoFile { get; set; } = null!; // VideoFile      
    }
}
