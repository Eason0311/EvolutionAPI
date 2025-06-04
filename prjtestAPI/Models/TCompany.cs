using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TCompany
{
    public int CompanyId { get; set; }

    public string Name { get; set; } = null!;

    public string? ContactEmail { get; set; }

    public DateTime? ContractStartAt { get; set; }

    public DateTime? ContractEndAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<TUser> TUsers { get; set; } = new List<TUser>();
}
