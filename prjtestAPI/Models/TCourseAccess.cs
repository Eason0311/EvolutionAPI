using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TCourseAccess
{
    public int CourseAccessId { get; set; }

    public int? UserId { get; set; }

    public int? CourseId { get; set; }

    public virtual TCourse? Course { get; set; }

    public virtual TUser? User { get; set; }
}
