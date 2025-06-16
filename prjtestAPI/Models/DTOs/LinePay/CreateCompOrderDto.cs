namespace prjEvolutionAPI.Models.DTOs.LinePay
{
    public class CreateCompOrderDto
    {
        /// <summary>下單公司 ID（對應 TCompOrder.BuyerCompanyId）</summary>
        public int CompanyId { get; set; }

        /// <summary>欲購買的課程 ID（對應 TCompOrder.CourseId）</summary>
        public int CourseId { get; set; }
    }
}
