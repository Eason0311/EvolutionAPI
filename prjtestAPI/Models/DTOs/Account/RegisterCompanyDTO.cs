using System.ComponentModel.DataAnnotations;

namespace prjEvolutionAPI.Models.DTOs.Account
{
    public class RegisterCompanyDTO
    {
        [Required(ErrorMessage = "CompanyName 為必填")]
        public string CompanyName { get; set; } = null!;
        [Required(ErrorMessage = "Email 為必填")]
        [EmailAddress(ErrorMessage = "請輸入有效的 Email 格式")]
        public string Email { get; set; } = null!;
    }
}
