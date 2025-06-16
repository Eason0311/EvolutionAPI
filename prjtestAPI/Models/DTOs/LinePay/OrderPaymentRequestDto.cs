namespace prjEvolutionAPI.Models.DTOs.LinePay
{
    public class OrderPaymentRequestDto
    {
        public bool IsCompanyOrder { get; set; }
        public int PartyId { get; set; }
        public int CourseId { get; set; }
        public string ProductName { get; set; }
    }
}
