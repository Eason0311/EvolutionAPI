using System;
using System.Collections.Generic;

namespace prjtestAPI.Models;

public partial class TRefreshToken
{
    public int TokenId { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public bool IsRevoked { get; set; }

    public virtual TUser User { get; set; } = null!;
}
