using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using prjEvolutionAPI.Hubs;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ChapterController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChapterService _chapterService;
        private readonly IHubContext<CourseHub> _hubContext;
        public ChapterController(IHubContext<CourseHub> hubContext, IUnitOfWork unitOfWork, IChapterService chapterService)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _chapterService = chapterService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChapterById(int id)
        {
            var chapter = await _unitOfWork.Chapters.GetByIdAsync(id);
            if (chapter == null) return NotFound("找不到章節");

            var result = new ResChapterDTO
            {
                ChapterID = chapter.ChapterId,
                CourseId = chapter.CourseId,
                ChapterTitle = chapter.ChapterTitle,
                ChapterDes = chapter.ChapterDes
            };
            return Ok(ApiResponse<ResChapterDTO>.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<IActionResult> CreateChapter([FromBody] VChapterDTO dto)
        {
            var ConnectionId = dto.ConnectionId;
            var chapter = await _chapterService.CreateChapterAsync(dto, ConnectionId, _hubContext);
            return Ok(ApiResponse<int>.SuccessResponse(chapter));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChapter(int id, [FromBody] VChapterDTO dto)
        {
            var ConnectionId = dto.ConnectionId;
            var updated = await _chapterService.UpdateChapterAsync(id, dto, ConnectionId, _hubContext);
            if (updated == null)
                return NotFound("找不到要更新的章節");

            return StatusCode(200, ApiResponse<bool>.SuccessResponse(updated, "章節更新成功"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            var result = await _chapterService.DeleteChapterAsync(id);
            if (!result) return NotFound("找不到要刪除的章節");

            return StatusCode(200, ApiResponse<string>.SuccessResponse("章節已刪除",200));
        }
    }
}
