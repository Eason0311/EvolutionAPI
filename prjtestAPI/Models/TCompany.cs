using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TCompany
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? CompanyPhone { get; set; }

    public string? CompanyAddress { get; set; }

    public string? CompanyNotice { get; set; }

    public string CompanyEmail { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TCompOrder> TCompOrders { get; set; } = new List<TCompOrder>();

    public virtual ICollection<TCourse> TCourses { get; set; } = new List<TCourse>();

    public virtual ICollection<TDepList> TDepLists { get; set; } = new List<TDepList>();

    public virtual ICollection<TUser> TUsers { get; set; } = new List<TUser>();
}
