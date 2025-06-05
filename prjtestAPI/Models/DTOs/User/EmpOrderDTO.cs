namespace prjEvolutionAPI.Models.DTOs.User
{
    public class EmpOrderDTO
    {
        public int CourseId { get; set; }

        public DateTime? OrderDate { get; set; }

        public int? Amount { get; set; }
    }
}
