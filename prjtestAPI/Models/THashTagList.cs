using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class THashTagList
{
    public int TagId { get; set; }

    public string TagName { get; set; } = null!;

    public virtual ICollection<TCourseHashTag> TCourseHashTags { get; set; } = new List<TCourseHashTag>();
}
