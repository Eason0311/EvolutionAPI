namespace prjEvolutionAPI.Models.DTOs.Publisher
{
    public class CompanyUpdateDTO
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = null!;

        public string CompanyEmail { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
