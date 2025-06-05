using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TOption
{
    public int OptionId { get; set; }

    public int QuestionId { get; set; }

    public string OptionText { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public virtual TQuestion Question { get; set; } = null!;

    public virtual ICollection<TQuizAnswerDetail> TQuizAnswerDetails { get; set; } = new List<TQuizAnswerDetail>();
}
