using System;
using System.Collections.Generic;

namespace prjtestAPI.Models;

public partial class TUserActionToken
{
    public int TokenId { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public string TokenType { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual TUser User { get; set; } = null!;
}
