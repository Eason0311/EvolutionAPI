using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TCourseAssignment
{
    public int AssignmentId { get; set; }

    public int UserId { get; set; }

    public int CourseId { get; set; }

    public DateTime? DueDate { get; set; }

    public int? MinScore { get; set; }

    public bool? IsPassed { get; set; }

    public DateTime? LastAttemptAt { get; set; }

    public virtual TCourse Course { get; set; } = null!;

    public virtual TUser User { get; set; } = null!;
}
