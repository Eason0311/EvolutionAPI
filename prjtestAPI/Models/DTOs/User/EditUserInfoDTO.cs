using System.ComponentModel.DataAnnotations;

namespace prjEvolutionAPI.Models.DTOs.User
{
    public class EditUserInfoDTO
    {
        [Required]
        public string Username { get; set; }
        public string UserEmail { get; set; }

        public string Department { get; set; }

        /// <summary>
        /// 使用者上傳的新頭像 (可選)
        /// </summary>
        public IFormFile? PhotoFile { get; set; }

    }
}
