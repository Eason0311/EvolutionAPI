using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TCourseChapter
{
    public int ChapterId { get; set; }

    public int CourseId { get; set; }

    public string ChapterTitle { get; set; } = null!;

    public string? ChapterDes { get; set; }

    public virtual TCourse Course { get; set; } = null!;

    public virtual ICollection<TVideo> TVideos { get; set; } = new List<TVideo>();
}
