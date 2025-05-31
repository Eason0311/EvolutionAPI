namespace prjtestAPI.Models.DTOs.Account
{
    public class ResetPasswordDTO
    {
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
