using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class SHashTagListService : IHashTagListService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SHashTagListService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ResHashTagDTO>> GetAllHashTagsAsync()
        {
            return await _unitOfWork.HashTagList.GetAllHashTagsAsync();
        }
    }
}
