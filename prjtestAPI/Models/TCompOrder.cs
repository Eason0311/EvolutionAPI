using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TCompOrder
{
    public int OrderId { get; set; }

    public int BuyerCompanyId { get; set; }

    public int CourseId { get; set; }

    public DateTime? OrderDate { get; set; }

    public int? Amount { get; set; }

    public bool? IsPaid { get; set; }

    public virtual TCompany BuyerCompany { get; set; } = null!;

    public virtual TCourse Course { get; set; } = null!;
}
