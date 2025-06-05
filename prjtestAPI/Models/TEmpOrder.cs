using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TEmpOrder
{
    public int OrderId { get; set; }

    public int BuyerUserId { get; set; }

    public int CourseId { get; set; }

    public DateTime? OrderDate { get; set; }

    public int? Amount { get; set; }

    public bool? IsPaid { get; set; }

    public virtual TUser BuyerUser { get; set; } = null!;

    public virtual TCourse Course { get; set; } = null!;
}
