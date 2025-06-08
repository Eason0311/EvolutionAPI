using prjEvolutionAPI.Models.DTOs.Course;
using prjEvolutionAPI.Models.DTOs.Home;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class HomeService : IHomeService
    {
        private readonly IUnitOfWork _uow;
        public HomeService(IUnitOfWork uow) 
        {
            _uow = uow;
        }

        public async Task<AboutInfoDTO> GetAboutInfoAsync()
        {
            return new AboutInfoDTO
            {
                CompanyCount = await _uow.Company.GetCompanyCountAsync(),
                UserCount = await _uow.Users.GetUserCountAsync(),
                CourseCount = await _uow.Course.GetCourseCountAsync(),
                QuizCount = await _uow.QuizResults.GetQuizResultsCountAsync()
            };
        }

        public async Task<List<HashTagListDTO>> GetRandomTagAsync()
        {
            return await _uow.HashTagList.GetRandomTagAsync();
        }
    }
}
