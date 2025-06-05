using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TQuizAnswerDetail
{
    public int AnswerDetailId { get; set; }

    public int ResultId { get; set; }

    public int QuestionId { get; set; }

    public int SelectedOptionId { get; set; }

    public virtual TQuestion Question { get; set; } = null!;

    public virtual TQuizResult Result { get; set; } = null!;

    public virtual TOption SelectedOption { get; set; } = null!;
}
