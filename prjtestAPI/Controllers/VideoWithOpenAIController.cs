using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoWithOpenAIController : ControllerBase
    {
        private readonly IOpenAIService _openAIService;
        public VideoWithOpenAIController(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }
        [HttpGet("{videoId}")]
        public async Task<IActionResult> AnalyzeVideo(int videoId)
        {
            if (videoId <= 0)
            {
                return BadRequest("無效的影片 ID");
            }
            var videoAItext = await _openAIService.TranscribeAudioAsync(videoId);
            var ResAItext = "";
            if (videoAItext == "FileTooLarge")
            {
                return BadRequest(ApiResponse<string>.FailResponse("選擇的影片過大，無法翻譯"));
            }
            ResAItext = await _openAIService.GenerateSummaryAsync(videoAItext);
            //string FakeResAItext = "\"1. 課程開始前，老師提醒學生若有錄音問題可隨時詢問。\\n2. 老師介紹了變數宣告及印出方法，包括整數（INT）和字串（STRING）的使用。\\n3. 學生提出對重複寫法的疑問，老師示範如何使用加法串接變數。\\n4. 討論整數（INT）的上限，並演示超過範圍時的錯誤行為。\\n5. 介紹了長整數（LONG）用於存放更大的數據，以避免整數溢出問題。\\n6. 強調編程時選擇合適的變數類型的重要性。\\n7. 示範如何在變數宣告和指派值的過程中合併步驟簡化寫法。\\n8. 課程結尾，老師鼓勵學生回去練習所學內容，並開放提問。\"";
            return Ok(ApiResponse<string>.SuccessResponse(ResAItext, "AI分析成功"));
        }
        
    }
}
