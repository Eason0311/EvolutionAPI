using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TQuestion
{
    public int QuestionId { get; set; }

    public int QuizId { get; set; }

    public string QuestionText { get; set; } = null!;

    public virtual TQuiz Quiz { get; set; } = null!;

    public virtual ICollection<TOption> TOptions { get; set; } = new List<TOption>();

    public virtual ICollection<TQuizAnswerDetail> TQuizAnswerDetails { get; set; } = new List<TQuizAnswerDetail>();
}
