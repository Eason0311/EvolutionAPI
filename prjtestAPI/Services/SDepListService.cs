using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class SDepListService : IDepListService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SDepListService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ResDepListDTO>> GetAllDepsAsync()
        {
            return await _unitOfWork.DepLists.GetAllDepartmentsAsync();
        }
    }
}
