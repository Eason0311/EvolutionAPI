using Microsoft.AspNetCore.SignalR;
using prjEvolutionAPI.Hubs;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface IVideoService
    {
        Task<TVideo> GetByIdAsync(int id);

        Task<IEnumerable<TVideo>> GetAllAsync();

        Task<int> AddVideoAsync(VVideoDTO dto, string userId, IHubContext<CourseHub> hubContext);

        Task<bool> UpdateVideoAsync(int id, VVideoDTO dto, string ConnectionId, IHubContext<CourseHub> hubContext);

        Task<bool> DeleteVideoAsync(int id);
    }
}
