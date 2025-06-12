using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TPayment
{
    public int PaymentId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public long? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual ICollection<TPaymentDetail> TPaymentDetails { get; set; } = new List<TPaymentDetail>();
}
