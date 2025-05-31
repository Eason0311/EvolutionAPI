using System;
using System.Collections.Generic;

namespace prjtestAPI.Models;

public partial class TUser
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? Role { get; set; }

    public string? UserStatus { get; set; }

    public int? FailedLoginCount { get; set; }

    public DateTime? LockoutEndTime { get; set; }

    public int CompanyId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public virtual TCompany Company { get; set; } = null!;

    public virtual ICollection<TRefreshToken> TRefreshTokens { get; set; } = new List<TRefreshToken>();

    public virtual ICollection<TUserActionToken> TUserActionTokens { get; set; } = new List<TUserActionToken>();
}
