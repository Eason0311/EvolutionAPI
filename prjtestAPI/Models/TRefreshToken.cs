﻿using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TRefreshToken
{
    public int TokenId { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public string IpAddress { get; set; } = null!;

    public string UserAgent { get; set; } = null!;

    public bool IsRevoked { get; set; }

    public virtual TUser User { get; set; } = null!;
}
