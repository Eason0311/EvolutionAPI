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
        public async Task<IEnumerable<ResDepListDTO>> GetAllDepsAsync(int userId)
        {
            var company = await _unitOfWork.Company.GetByUserIdAsync(userId);
            if (company == null)
            {
                throw new Exception("Company not found for the given user.");
            }
            var companyId = company.CompanyId;
            return await _unitOfWork.DepList.GetAllDepartmentsAsync(companyId);
        }
    }
}
