using prjEvolutionAPI.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xabe.FFmpeg;
namespace prjEvolutionAPI.Services
{
    public class SOpenAIService : IOpenAIService
    {
        private readonly HttpClient _paidClient;
        private readonly HttpClient _assemblyAIClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SOpenAIService> _logger;

        public SOpenAIService(IConfiguration config, IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork, ILogger<SOpenAIService> logger)
        {
            _paidClient = httpClientFactory.CreateClient("PaidOpenAI");
            _assemblyAIClient = httpClientFactory.CreateClient("AssemblyAI");
            _unitOfWork = unitOfWork;
            // 只設定已下載好的路徑
            FFmpeg.SetExecutablesPath(@"C:\ffmpeg\bin");
            _logger = logger;
        }
        //付費OpenAI API 

        //付費OpenAI API 音檔轉文字
        public async Task<string> TranscribeAudioAsync(int videoId)
        {
            var video = await _unitOfWork.Videos.GetByIdAsync(videoId);
            string videoFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
            string videoPath = Path.Combine(videoFolder, video.VideoUrl);
            string audioPath = Path.ChangeExtension(videoPath, ".mp3");

            if (!File.Exists(audioPath))
            {
                await ExtractAudioAsync(videoPath);
            }

            var fileInfo = new FileInfo(audioPath);
            const long maxSize = 25 * 1024 * 1024;
            if (fileInfo.Length > maxSize)
            {
                TryDeleteFile(audioPath);
                return "FileTooLarge";
            }

            try
            {
                using var fileStream = File.OpenRead(audioPath);
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
                content.Add(fileContent, "file", Path.GetFileName(audioPath));
                content.Add(new StringContent("whisper-1"), "model");

                using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/transcriptions")
                {
                    Content = content
                };

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(100));
                var response = await _paidClient.SendAsync(request, cts.Token);

                _logger.LogInformation("[Whisper] Status: {StatusCode}", response.StatusCode);
                string json = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("[Whisper] Response body: {Body}", json);

                if (!response.IsSuccessStatusCode)
                {
                    return $"Error: {(int)response.StatusCode} - {json}";
                }

                using var doc = JsonDocument.Parse(json);
                var transcript = doc.RootElement.GetProperty("text").GetString() ?? "";
                return transcript;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "[Whisper] 請求逾時或被取消");
                return "RequestTimeout";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Whisper] 發送或處理失敗");
                return "RequestFailed";
            }
            finally
            {
                TryDeleteFile(audioPath);
            }
        }
        //刪除音檔
        private bool TryDeleteFile(string filePath)
        {
            const int maxRetries = 3;
            const int delayMilliseconds = 100;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath); // 同步、阻塞
                        _logger.LogInformation("已刪除音訊檔案: {Path}", filePath);
                    }
                    return true;
                }
                catch (IOException ex) when (attempt < maxRetries)
                {
                    _logger.LogWarning(ex, "第 {Attempt} 次刪除失敗，將重試: {Path}", attempt, filePath);
                    Thread.Sleep(delayMilliseconds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "未知錯誤導致刪除失敗: {Path}", filePath);
                    return false;
                }
            }

            _logger.LogWarning("最終仍無法刪除檔案: {Path}", filePath);
            return false;
        }
        // 付費OpenAI API 影片文字稿摘要
        public async Task<string> GenerateSummaryAsync(string transcript)
        {
            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
            new { role = "system", content = "你是一位教學影片的內容摘要專家，請根據使用者提供的影片文字稿製作一份中文大綱摘要。" },
            new { role = "user", content = $"以下是影片文字內容：\n\n{transcript}\n\n請幫我產出內容摘要，大約 5~10 行，條列重點。\r\n" }
        },
                temperature = 0.7
            };

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
                {
                    Content = new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json")
                };

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(100));
                var response = await _paidClient.SendAsync(request, cts.Token);

                _logger.LogInformation("[ChatGPT] Status: {StatusCode}", response.StatusCode);
                string json = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("[ChatGPT] Response body: {Body}", json);

                if (!response.IsSuccessStatusCode)
                {
                    return $"Error: {(int)response.StatusCode} - {json}";
                }

                using var doc = JsonDocument.Parse(json);
                var result = doc.RootElement
                                .GetProperty("choices")[0]
                                .GetProperty("message")
                                .GetProperty("content")
                                .GetString();

                return result ?? "";
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "[ChatGPT] 請求逾時或被取消");
                return "RequestTimeout";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ChatGPT] 發送或處理失敗");
                return "RequestFailed";
            }
        }
        //付費 影片轉音檔 低於25MB
        private async Task ExtractAudioAsync(string videoFilePath)
        {
            string audioPath = Path.ChangeExtension(videoFilePath, ".mp3");

            _logger.LogInformation("開始轉檔: {Path}", videoFilePath);
            _logger.LogInformation("輸出音訊: {Path}", audioPath);

            var conversion = await FFmpeg.Conversions.FromSnippet.ExtractAudio(videoFilePath, audioPath);

            try
            {
                await conversion.Start();
                if (File.Exists(audioPath))
                    _logger.LogInformation("音訊轉檔成功！");
                else
                    _logger.LogWarning("音訊檔未建立");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "轉檔失敗");
            }

        }

    }
}
