using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TCourseHashTag
{
    public int CourseHashTagId { get; set; }

    public int CourseId { get; set; }

    public int TagId { get; set; }

    public virtual TCourse Course { get; set; } = null!;

    public virtual THashTagList Tag { get; set; } = null!;
}
