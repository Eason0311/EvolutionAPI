namespace prjEvolutionAPI.Models.DTOs.LinePay
{
    public class OrderItemDto
    {
        public int CourseId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
