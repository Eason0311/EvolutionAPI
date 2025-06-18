using Microsoft.AspNetCore.SignalR;
using prjEvolutionAPI.Helpers;
using prjEvolutionAPI.Hubs;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class SVideoService : IVideoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SVideoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TVideo> GetByIdAsync(int id)
        {
            return await _unitOfWork.Videos.GetByIdAsync(id);
        }

        public async Task<IEnumerable<TVideo>> GetAllAsync()
        {
            return await _unitOfWork.Videos.GetAllAsync();
        }

        public async Task<int> AddVideoAsync(VVideoDTO dto, string ConnectionId, IHubContext<CourseHub> hubContext)
        {
            if (dto.VideoFile == null || dto.VideoFile.Length == 0)
                throw new ArgumentException("影片檔案不可為空", nameof(dto.VideoFile));

            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Video:Started",
                Data = new ProgressData { Percent = 20, Message = "開始新增影片" },
                clientRequestId = dto.clientRequestId
            });

            // 先產生唯一檔名
            string newVideoFileName = GetGuidFileName(dto.VideoFile);

            var video = new TVideo
            {
                ChapterId = (int)dto.ChapterId,
                VideoTitle = dto.Title,
                VideoUrl = newVideoFileName,  // 影片檔名先存入資料庫
            };

            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Video:SavingToDb",
                Data = new ProgressData { Percent = 30, Message = "寫入影片資料庫中" },
                clientRequestId = dto.clientRequestId
            });

            await _unitOfWork.Videos.AddAsync(video);
            await _unitOfWork.CompleteAsync();

            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Video:SavingFile",
                Data = new ProgressData { Percent = 60, Message = "開始儲存影片檔案" },
                clientRequestId = dto.clientRequestId
            });

            // 儲存影片檔案到伺服器
            await SaveVideoAsync(dto.VideoFile, newVideoFileName);

            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Video:Completed",
                Data = new ProgressData { Percent = 100, Message = "影片新增完成" },
                clientRequestId = dto.clientRequestId
            });

            return video.VideoId;
        }

        private string GetGuidFileName(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("影片檔案不可為空", nameof(file));
            }

            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(fileName).ToLower();
            var allowedExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".mkv" };

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("檔案類型錯誤，僅支援 .mp4, .avi, .mov, .wmv, .mkv 格式", nameof(file));
            }

            var contentType = file.ContentType.ToLower();
            var allowedMimeTypes = new[]
            {
                "video/mp4",
                "video/x-msvideo",     // avi
                "video/quicktime",     // mov
                "video/x-ms-wmv",      // wmv
                "video/x-matroska"     // mkv
            };

            if (!allowedMimeTypes.Contains(contentType))
            {
                throw new ArgumentException($"不支援的影片格式（MIME 類型為 {contentType}）", nameof(file));
            }

            var guid = Guid.NewGuid();
            return $"{guid}{fileExtension}";
        }


        private string GetVideoFolderPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
        }

        private string GetVideoFullPath(string fileName)
        {
            return Path.Combine(GetVideoFolderPath(), fileName);
        }

        private async Task SaveVideoAsync(IFormFile file, string fileName)
        {
            var folderPath = GetVideoFolderPath();
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = GetVideoFullPath(fileName);
            // 如果檔案已存在先刪除
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }

        private void DeleteOldFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                for (int i = 0; i < 3; i++) // 最多重試 3 次
                {
                    try
                    {
                        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            fs.Close();
                        }

                        File.Delete(filePath);
                        break; // 刪除成功就跳出
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(200); // 等一下再試
                    }
                }
            }
        }

        public async Task<bool> UpdateVideoAsync(int id, VVideoDTO dto, string ConnectionId, IHubContext<CourseHub> hubContext)
        {
            var video = await _unitOfWork.Videos.GetByIdAsync(id);
            if (video == null) throw new ArgumentException("找不到指定的影片", nameof(id));

            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Video:Started",
                Data = new ProgressData { Percent = 30, Message = "開始更新影片" },
                clientRequestId = dto.clientRequestId
            });

            if (dto.VideoFile != null && dto.VideoFile.Length > 0)
            {
                // 如果有新的影片檔案，先刪除舊檔案
                var oldFilePath = GetVideoFullPath(video.VideoUrl);
                DeleteOldFile(oldFilePath);
                // 產生新的檔名
                string newVideoFileName = GetGuidFileName(dto.VideoFile);
                video.VideoUrl = newVideoFileName; // 更新影片檔名
                video.VideoTitle = dto.Title;
                await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
                {
                    Step = "Video:SavingToDb",
                    Data = new ProgressData { Percent = 70, Message = "寫入影片資料庫中..." },
                    clientRequestId = dto.clientRequestId
                });

                _unitOfWork.Videos.Update(video);
                await _unitOfWork.CompleteAsync();
                await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
                {
                    Step = "Video:SavingFile",
                    Data = new ProgressData { Percent = 90, Message = "儲存影片檔案中..." },
                    clientRequestId = dto.clientRequestId
                });
                // 儲存新的影片檔案
                await SaveVideoAsync(dto.VideoFile, newVideoFileName);
                await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
                {
                    Step = "Video:Completed",
                    Data = new ProgressData { Percent = 100, Message = "影片更新完成" },
                    clientRequestId = dto.clientRequestId
                });
            }
            else
            {
                // 如果沒有新的影片檔案，只更新標題
                video.VideoTitle = dto.Title;
                _unitOfWork.Videos.Update(video);
                await _unitOfWork.CompleteAsync();
                await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
                {
                    Step = "Video:Completed",
                    Data = new ProgressData { Percent = 100, Message = "影片標題更新完成" },
                    clientRequestId = dto.clientRequestId
                });
            }
            return true;
        }

        public async Task<bool> DeleteVideoAsync(int id)
        {
            var video = await _unitOfWork.Videos.GetByIdAsync(id);
            if (video == null) throw new ArgumentException("找不到指定的影片", nameof(id));


            // 刪除影片檔案
            var videoFilePath = GetVideoFullPath(video.VideoUrl);
            DeleteOldFile(videoFilePath);
            _unitOfWork.Videos.Delete(video);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
