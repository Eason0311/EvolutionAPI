using System;
using System.Collections.Generic;

namespace prjEvolutionAPI.Models;

public partial class TPaymentDetail
{
    public int DetailId { get; set; }

    public int PaymentId { get; set; }

    public int? CompOrderId { get; set; }

    public int? EmpOrderId { get; set; }

    public virtual TCompOrder? CompOrder { get; set; }

    public virtual TEmpOrder? EmpOrder { get; set; }

    public virtual TPayment Payment { get; set; } = null!;
}
