using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TVideo
{
    public int VideoId { get; set; }

    public int ChapterId { get; set; }

    public string? VideoTitle { get; set; }

    public string? VideoUrl { get; set; }

    public virtual TCourseChapter Chapter { get; set; } = null!;
}
