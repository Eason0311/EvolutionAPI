using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TDepList
{
    public int DepId { get; set; }

    public string DepName { get; set; } = null!;

    public int CompanyId { get; set; }

    public virtual TCompany Company { get; set; } = null!;

    public virtual ICollection<TUser> TUsers { get; set; } = new List<TUser>();
}
