namespace prjtestAPI.Models.DTOs.Account
{
    public class InitPasswordDTO
    {
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
