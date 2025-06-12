namespace prjEvolutionAPI.Models.DTOs.LinePay
{
    public class PaymentRequestDto
    {
        public decimal Amount { get; set; }
        public string OrderId { get; set; } = null!;
        public string ProductName { get; set; } = null!;
    }
}
