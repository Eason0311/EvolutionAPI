using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TCourse
{
    public int CourseId { get; set; }

    public int CompanyId { get; set; }

    public string CourseTitle { get; set; } = null!;

    public string? CourseDes { get; set; }

    public bool IsPublic { get; set; }

    public string CoverImagePath { get; set; } = null!;

    public int Price { get; set; }

    public bool IsDraft { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual TCompany Company { get; set; } = null!;

    public virtual ICollection<TCompOrder> TCompOrders { get; set; } = new List<TCompOrder>();

    public virtual ICollection<TCourseAccess> TCourseAccesses { get; set; } = new List<TCourseAccess>();

    public virtual ICollection<TCourseAssignment> TCourseAssignments { get; set; } = new List<TCourseAssignment>();

    public virtual ICollection<TCourseChapter> TCourseChapters { get; set; } = new List<TCourseChapter>();

    public virtual ICollection<TCourseHashTag> TCourseHashTags { get; set; } = new List<TCourseHashTag>();

    public virtual ICollection<TEmpOrder> TEmpOrders { get; set; } = new List<TEmpOrder>();

    public virtual ICollection<TQuiz> TQuizzes { get; set; } = new List<TQuiz>();
}
