namespace prjEvolutionAPI.Models.DTOs.User
{
    public class UserInfoDTO
    {
        public int UserId { get; set; }       // 如果你想前端拿到使用者 ID
        public string Username { get; set; } = null!;
        public string UserCompany { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DepName { get; set; } = null!;
        public string? UserPicPath { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
