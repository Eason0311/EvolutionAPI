using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TUser
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? UserPic { get; set; }

    public int UserDep { get; set; }

    public string Role { get; set; } = null!;

    public string UserStatus { get; set; } = null!;

    public int FailedLoginCount { get; set; }

    public DateTime? LockoutEndTime { get; set; }

    public int CompanyId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public virtual TCompany Company { get; set; } = null!;

    public virtual ICollection<TCourseAccess> TCourseAccesses { get; set; } = new List<TCourseAccess>();

    public virtual ICollection<TCourseAssignment> TCourseAssignments { get; set; } = new List<TCourseAssignment>();

    public virtual ICollection<TEmpOrder> TEmpOrders { get; set; } = new List<TEmpOrder>();

    public virtual ICollection<TQuizResult> TQuizResults { get; set; } = new List<TQuizResult>();

    public virtual ICollection<TRefreshToken> TRefreshTokens { get; set; } = new List<TRefreshToken>();

    public virtual ICollection<TUserActionToken> TUserActionTokens { get; set; } = new List<TUserActionToken>();

    public virtual TDepList UserDepNavigation { get; set; } = null!;
}
