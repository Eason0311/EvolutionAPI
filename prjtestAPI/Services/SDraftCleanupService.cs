using Microsoft.Extensions.Logging;

namespace prjEvolutionAPI.Services
{
    public class SDraftCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SDraftCleanupService> _logger;

        public SDraftCleanupService(IServiceProvider serviceProvider, ILogger<SDraftCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("草稿課程清理服務已啟動");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<DbTestCourseContext>();
                        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                        var draftCourses = await context.TCourses
                                                .Include(c => c.TCourseChapters)
                                                    .ThenInclude(ch => ch.TVideos)
                                                .Include(c => c.TCourseHashTags)
                                                .Include(c => c.TCourseAccesses) // <<< 新增這一行
                                                .Where(c => c.IsDraft)
                                                .ToListAsync(stoppingToken);


                        if (draftCourses.Count > 0)
                        {
                            _logger.LogInformation("準備刪除 {Count} 筆草稿課程資料", draftCourses.Count);

                            foreach (var course in draftCourses)
                            {
                                // 刪除封面圖片（如有）
                                if (!string.IsNullOrEmpty(course.CoverImagePath))
                                {
                                    var coverPath = Path.Combine(env.WebRootPath, "images", course.CoverImagePath);
                                    if (File.Exists(coverPath))
                                    {
                                        File.Delete(coverPath);
                                        _logger.LogInformation("已刪除封面圖：{Path}", coverPath);
                                    }
                                }

                                // 刪除所有章節影片檔案（如有）
                                foreach (var chapter in course.TCourseChapters)
                                {
                                    foreach (var video in chapter.TVideos)
                                    {
                                        if (!string.IsNullOrEmpty(video.VideoUrl))
                                        {
                                            var videoPath = Path.Combine(env.WebRootPath, "videos", video.VideoUrl);
                                            if (File.Exists(videoPath))
                                            {
                                                File.Delete(videoPath);
                                                _logger.LogInformation("已刪除影片：{Path}", videoPath);
                                            }
                                        }
                                    }
                                }
                            }

                            // 最後刪除資料庫資料
                            context.TCourses.RemoveRange(draftCourses);
                            await context.SaveChangesAsync(stoppingToken);

                            _logger.LogInformation("成功刪除草稿課程與檔案");
                        }
                        else
                        {
                            _logger.LogInformation("目前無需清理的草稿課程");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "刪除草稿課程時發生例外錯誤：{Message}", ex.Message);
                }

                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }

            _logger.LogInformation("草稿課程清理服務已停止");
        }

    }
}
