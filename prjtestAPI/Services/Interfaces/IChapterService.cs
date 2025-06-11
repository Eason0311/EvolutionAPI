using Microsoft.AspNetCore.SignalR;
using prjEvolutionAPI.Hubs;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Models;

namespace prjEvolutionAPI.Services.Interfaces
{
    public interface IChapterService
    {
        Task<int> CreateChapterAsync(VChapterDTO dto, string userId, IHubContext<CourseHub> hubContext);
        Task<TCourseChapter> GetChapterAsync(int id);
        Task<IEnumerable<TCourseChapter>> GetAllChaptersAsync();
        Task<bool> UpdateChapterAsync(int id, VChapterDTO dto, string ConnectionId, IHubContext<CourseHub> hubContext);
        Task<bool> DeleteChapterAsync(int id);
    }
}
