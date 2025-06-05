using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TQuiz
{
    public int QuizId { get; set; }

    public int CourseId { get; set; }

    public string Title { get; set; } = null!;

    public virtual TCourse Course { get; set; } = null!;

    public virtual ICollection<TQuestion> TQuestions { get; set; } = new List<TQuestion>();

    public virtual ICollection<TQuizResult> TQuizResults { get; set; } = new List<TQuizResult>();
}
