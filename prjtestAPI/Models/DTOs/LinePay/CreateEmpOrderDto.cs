namespace prjEvolutionAPI.Models.DTOs.LinePay
{
    public class CreateEmpOrderDto
    {
        /// <summary>下單使用者 ID（對應 TEmpOrder.BuyerUserId）</summary>
        public int UserId { get; set; }

        /// <summary>欲購買的課程 ID（對應 TEmpOrder.CourseId）</summary>
        public int CourseId { get; set; }
    }
}
