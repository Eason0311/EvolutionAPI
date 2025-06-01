namespace prjEvolutionAPI.Models.DTOs
{
    public class UserInfoDTO
    {
        public int UserId { get; set; }       // 如果你想前端拿到使用者 ID
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
