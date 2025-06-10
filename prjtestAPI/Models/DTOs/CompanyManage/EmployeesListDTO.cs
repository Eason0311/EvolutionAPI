namespace prjEvolutionAPI.Models.DTOs.CompanyManage
{
    public class EmployeesListDTO
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string UserDep { get; set; } = null!;

        public string UserStatus { get; set; } = null!;
    }
}
