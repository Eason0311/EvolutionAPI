namespace prjEvolutionAPI.Models.DTOs.Publisher
{
    public class CompanyCreateDTO
    {
        public string CompanyName { get; set; } = null!;

        public string CompanyEmail { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
