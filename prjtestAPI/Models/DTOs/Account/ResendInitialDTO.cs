using System.ComponentModel.DataAnnotations;

namespace prjEvolutionAPI.Models.DTOs.Account
{
    public class ResendInitialDTO
    {
        [Required(ErrorMessage = "Email 不可為空")]
        [EmailAddress(ErrorMessage = "請輸入有效的 Email 格式")]
        public string Email { get; set; } = null!;
    }
}
