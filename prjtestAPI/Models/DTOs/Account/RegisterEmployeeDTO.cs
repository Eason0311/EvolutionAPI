using System.ComponentModel.DataAnnotations;

namespace prjtestAPI.Models.DTOs.Account
{
    public class RegisterEmployeeDTO
    {
        [Required(ErrorMessage = "Email 為必填")]
        [EmailAddress(ErrorMessage = "請輸入有效的 Email 格式")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Username 為必填")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "部門名稱為必填")]
        public string DepName { get; set; } = null!;
    }
}
