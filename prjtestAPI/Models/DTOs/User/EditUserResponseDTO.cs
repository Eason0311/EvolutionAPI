namespace prjEvolutionAPI.Models.DTOs.User
{
    public class EditUserResponseDTO
    {
        public UserInfoDTO UserInfo { get; set; } = null!;
        public string NewAccessToken { get; set; } = null!;
    }
}
