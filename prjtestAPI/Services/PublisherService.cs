using prjEvolutionAPI.Models.DTOs.Publisher;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IUnitOfWork _uow;
        public PublisherService(IUnitOfWork uow) 
        { 
            _uow = uow;
        }

        public async Task<List<CompanyListDTO>> GetCompanyListAsync()
        {
            return await _uow.Publisher.GetAllAsync()
                   ?? new List<CompanyListDTO>();
        }
    }
}
