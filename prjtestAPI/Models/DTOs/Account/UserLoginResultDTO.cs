using prjEvolutionAPI.Models;

namespace prjtestAPI.Models.DTOs.Account
{
    public class UserLoginResultDTO
    {
        public bool EmailExists { get; set; }
        public bool PasswordValid { get; set; }
        public TUser? User { get; set; }
    }
}
