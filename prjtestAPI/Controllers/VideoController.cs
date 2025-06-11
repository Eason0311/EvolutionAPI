using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using prjEvolutionAPI.Helpers;
using prjEvolutionAPI.Hubs;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoService _videoService;
        private readonly IHubContext<CourseHub> _hubContext;

        public VideoController(
            IHubContext<CourseHub> hubContext,
            IUnitOfWork unitOfWork,
            IVideoService videoService)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _videoService = videoService;
        }

        // 取得單一影片資料
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideoById(int id)
        {
            var video = await _unitOfWork.Videos.GetByIdAsync(id);
            if (video == null)
                return NotFound(ApiResponse<string>.FailResponse("找不到影片"));

            var result = new ResVideoDTO
            {
                VideoID = video.VideoId,
                ChapterId = video.ChapterId,
                Title = video.VideoTitle,
                VideoFile = video.VideoUrl
            };

            return Ok(ApiResponse<ResVideoDTO>.SuccessResponse(result, "取得影片成功"));
        }

        // 新增影片，只回傳新建的影片ID
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateVideo([FromForm] VVideoDTO dto)
        {
            var ConnectionId = dto.ConnectionId;
            await _hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Video:Upload",
                Data = new ProgressData { Percent = 5, Message = "開始上傳影片" },
                clientRequestId = dto.clientRequestId
            });
            var newVideoId = await _videoService.AddVideoAsync(dto, ConnectionId, _hubContext);
            return Ok(ApiResponse<int>.SuccessResponse(newVideoId, "影片建立成功"));
        }

        // 更新影片，回傳 ApiResponse<bool>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateVideo(int id, [FromForm] VVideoDTO dto)
        {
            var ConnectionId = dto.ConnectionId;
            if (dto.VideoFile != null)
            {
                await _hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
                {
                    Step = "Video:Upload",
                    Data = new ProgressData { Percent = 20, Message = "開始上傳影片" },
                    clientRequestId = dto.clientRequestId
                });
            }
            var updated = await _videoService.UpdateVideoAsync(id, dto, ConnectionId, _hubContext);
            if (!updated)
                return NotFound(ApiResponse<bool>.FailResponse("找不到要更新的影片"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "影片更新成功"));
        }

        // 刪除影片
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {

            var result = await _videoService.DeleteVideoAsync(id);
            if (!result)
                return NotFound(ApiResponse<string>.FailResponse("找不到要刪除的影片"));

            return Ok(ApiResponse<string>.SuccessResponse("影片已刪除",200));
        }
    }
}
