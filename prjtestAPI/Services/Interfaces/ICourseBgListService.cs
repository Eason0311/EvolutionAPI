using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.CourseBgList;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface ICourseBgListService
    {
        Task<IEnumerable<ResCourseBgListDTO>> GetCoursePageAsync( int userId);

        Task<IEnumerable<ResUserBgListDTO>> GetEmployeeAsync(int userId);
    }
}

