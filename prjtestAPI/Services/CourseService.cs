using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using prjEvolutionAPI.Helpers;
using prjEvolutionAPI.Hubs;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Course;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Repositories.Interfaces;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMemoryCache _cache;
        public CourseService(ICourseRepository repo, IUnitOfWork uow, IMemoryCache cache)
        {
            _repo = repo;
            _uow = uow;
            _cache = cache;
        }

        public async Task<PagedResult<CourseDTO>> GetPagedAsync(int pageIndex, int pageSize)
        {
            var (itemsDto, totalCount) = await _repo.GetPagedAsync(pageIndex, pageSize);

            // 2. 直接把這個 DTO 清單丟到 PagedResult 裡
            return new PagedResult<CourseDTO>
            {
                Items = itemsDto.ToList(),  // or just itemsDto if PagedResult.Items is IEnumerable<T>
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public Task<IEnumerable<CourseDTO>> GetCoursesByIdsAsync(IEnumerable<int> ids)
    => _repo.GetByIdsAsync(ids);

        public async Task<List<string>> GetTitleSuggestionsAsync(string prefix)
        {
            if (prefix.Length < 2)
                return new List<string>();

            var key = $"Suggest_{prefix}";
            if (!_cache.TryGetValue(key, out List<string> list))
            {
                list = await _repo.GetTitleSuggestionsAsync(prefix, 10);
                _cache.Set(key, list, TimeSpan.FromMinutes(2));
            }
            return list;
        }

        public async Task<List<CourseDTO>> SearchAsync(string query)
        {
            // 可以不快取，亦可依需求加入
            return await _repo.SearchAsync(query);
        }

        private string GetImageFolderPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
        }

        private string GetImageFullPath(string fileName)
        {
            return Path.Combine(GetImageFolderPath(), fileName);
        }
        // 儲存封面圖片
        private async Task SaveImageAsync(IFormFile file, string fileName)
        {
            var folderPath = GetImageFolderPath();
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = GetImageFullPath(fileName);
            DeleteOldFile(filePath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }


        //刪除舊的檔案
        private void DeleteOldFile(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                // 可以用 logger 寫入真實環境紀錄，也可以加上 SignalR 通知錯誤
                //await hubContext.Clients.User(UserId).SendAsync("ReceiveProgress", 95, "⚠️ 刪除舊封面時發生錯誤，但不影響流程");

                Console.WriteLine($"❗刪除檔案失敗：{filePath}，錯誤訊息：{ex.Message}");
            }
        }
        public async Task<bool> MarkCourseAsCompletedAsync(int id, VFinalDTO finalDTO, IHubContext<CourseHub> hubContext)
        {
            var course = await _uow.CreateCourse.GetByIdAsync(id);
            if (course == null)
            {
                throw new ArgumentException("找不到指定的課程", nameof(id));
            }
            try
            {
                course.IsDraft = finalDTO.IsDraft; // 更新課程狀態為草稿或已完成
                this._uow.CreateCourse.Update(course);
                await _uow.CompleteAsync();
                return true;
            }
            catch (Exception)
            {

                throw new ArgumentException("確認課程失敗");
            }
        }
        private string GetGuidFileName(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("封面圖片不可為空", nameof(file));
            }

            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(fileName).ToLower();
            var contentType = file.ContentType.ToLower();

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var allowedContentTypes = new[] { "image/jpeg", "image/png" };

            if (!allowedExtensions.Contains(fileExtension) || !allowedContentTypes.Contains(contentType))
            {
                throw new ArgumentException("檔案類型錯誤，僅支援 jpg、png、jpeg", nameof(file));
            }

            var guid = Guid.NewGuid();
            var newFileName = $"{guid}{fileExtension}";
            return newFileName;
        }

        // 建立課程並回傳 CourseId
        public async Task<int> CreateCourseAsync(VCourseDTO dto, string ConnectionId,int CompanyId, IHubContext<CourseHub> hubContext)
        {
            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Course:Started",
                Data = new ProgressData
                {
                    Percent = 10,
                    Message = "開始建立課程"
                },
                clientRequestId = dto.clientRequestId
            });
            if (dto.CoverImage != null)
            {
                // 生成唯一的檔案名稱
                string newcoverImagePath = this.GetGuidFileName(dto.CoverImage);
                var course = new TCourse
                {
                    CompanyId = CompanyId,
                    CourseTitle = dto.CourseTitle,
                    CourseDes = dto.CourseDes,
                    IsPublic = dto.IsPublic,
                    CoverImagePath = Path.Combine("images", newcoverImagePath).Replace("\\", "/"),
                    Price = dto.Price,
                    IsDraft = true,
                    //CreatedAt = DateTime.UtcNow
                };
                await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
                {
                    Step = "Course:SavingToDb",
                    Data = new ProgressData
                    {
                        Percent = 30,
                        Message = "寫入資料庫中..."
                    },
                    clientRequestId = dto.clientRequestId
                });
                var addedCourse = await _uow.CreateCourse.AddAsync(course);
                await _uow.CompleteAsync();
                await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
                {
                    Step = "Course:SavingImage",
                    Data = new ProgressData
                    {
                        Percent = 60,
                        Message = "儲存封面圖片..."
                    },
                    clientRequestId = dto.clientRequestId
                });
                await this.SaveImageAsync(dto.CoverImage, newcoverImagePath); // 儲存封面圖片
                await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
                {
                    Step = "Course:Completed",
                    Data = new ProgressData
                    {
                        Percent = 100,
                        Message = "建立完成"
                    },
                    clientRequestId = dto.clientRequestId
                });
                return addedCourse.CourseId; // ✅ 回傳 CourseId 給 Controller
            }
            else
            {
                throw new ArgumentException("封面圖片不可為空", nameof(dto.CoverImage));
            }

        }

        // 根據 ID 取得課程
        public async Task<TCourse> GetCourseAsync(int id)
        {
            return await _uow.CreateCourse.GetByIdAsync(id);
        }

        // 取得所有課程
        public async Task<IEnumerable<TCourse>> GetAllCoursesAsync()
        {
            return await _uow.CreateCourse.GetAllAsync();
        }


        // 更新課程
        public async Task<bool> UpdateCourseAsync(int id, VCourseDTO dto, string ConnectionId, IHubContext<CourseHub> hubContext)
        {
            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Course:Started",
                Data = new ProgressData
                {
                    Percent = 10,
                    Message = "開始更新課程"
                },
                clientRequestId = dto.clientRequestId
            });

            var course = await _uow.CreateCourse.GetByIdAsync(id);
            if (course == null)
            {
                throw new ArgumentException("找不到指定的課程", nameof(id));
            }

            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Course:Processing",
                Data = new ProgressData
                {
                    Percent = 50,
                    Message = "處理課程資料中..."
                },
                clientRequestId = dto.clientRequestId
            });

            if (dto.CoverImage != null)
            {
                string newCoverImagePath = this.GetGuidFileName(dto.CoverImage);

                var oldCoverImagePath = GetImageFullPath(course.CoverImagePath);
                this.DeleteOldFile(oldCoverImagePath);

                await SaveImageAsync(dto.CoverImage, newCoverImagePath);
                course.CoverImagePath = newCoverImagePath;
            }

            course.CourseTitle = dto.CourseTitle;
            course.CourseDes = dto.CourseDes;
            course.IsPublic = dto.IsPublic;
            course.Price = dto.Price;

            _uow.CreateCourse.Update(course);
            await _uow.CompleteAsync();

            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Course:Completed",
                Data = new ProgressData
                {
                    Percent = 100,
                    Message = "課程更新完成"
                },
                clientRequestId = dto.clientRequestId
            });

            return true;
        }

        // 刪除課程
        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _uow.CreateCourse.GetByIdAsync(id);
            if (course == null) return false;
            // 刪除封面圖片
            var coverImagePath = GetImageFullPath(course.CoverImagePath);
            this.DeleteOldFile(coverImagePath); // 刪除舊的檔案
            _uow.CreateCourse.Delete(course);
            await _uow.CompleteAsync();
            return true;
        }
        public async Task<ResFinalCourse> GetCourseLearning(int courseId)
        {
            TCourse course = await _uow.CreateCourse.GetByIdAsync(courseId);
            var chapters= await _uow.CreateCourse.GetChaptersWithVideosByCourseIdAsync(courseId);
            ResFinalCourse resFinalCourse = new ResFinalCourse{
                CourseTitle=course.CourseTitle,
                CourseDes = course.CourseDes,
                CoverImagePath = course.CoverImagePath,
                Price = course.Price,
                IsPublic = course.IsPublic,
                chapterWithVideos = chapters.Select(c => new ResChapterWithVideo
                {
                    ChapterTitle = c.ChapterTitle,
                    ChapterDes = c.ChapterDes,
                    videos = c.TVideos.Select(v => new ResFinalVideo
                    {
                        VideoId = v.VideoId,
                        VideoTitle = v.VideoTitle,
                        VideoFile = v.VideoUrl
                    }).ToList()
                }).ToList()

            };
            return resFinalCourse;
        }
    }
}
