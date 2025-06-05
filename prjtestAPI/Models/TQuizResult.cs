using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TQuizResult
{
    public int ResultId { get; set; }

    public int QuizId { get; set; }

    public int UserId { get; set; }

    public int? Score { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual TQuiz Quiz { get; set; } = null!;

    public virtual ICollection<TQuizAnswerDetail> TQuizAnswerDetails { get; set; } = new List<TQuizAnswerDetail>();

    public virtual TUser User { get; set; } = null!;
}
