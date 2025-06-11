using Microsoft.AspNetCore.SignalR;
using prjEvolutionAPI.Helpers;
using prjEvolutionAPI.Hubs;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Services.Interfaces;

namespace prjEvolutionAPI.Services
{
    public class SChapterService : IChapterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SChapterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateChapterAsync(VChapterDTO dto, string ConnectionId, IHubContext<CourseHub> hubContext)
        {
            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Chapter:Started",
                Data = new ProgressData { Percent = 10, Message = "開始新增章節" },
                clientRequestId = dto.clientRequestId
            });

            var chapter = new TCourseChapter
            {
                CourseId = dto.CourseId,
                ChapterTitle = dto.ChapterTitle,
                ChapterDes = dto.ChapterDes
            };

            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Chapter:SavingToDb",
                Data = new ProgressData { Percent = 50, Message = "寫入資料庫中..." },
                clientRequestId = dto.clientRequestId
            });

            var addedChapter = await _unitOfWork.Chapters.AddAsync(chapter);
            await _unitOfWork.CompleteAsync();

            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Chapter:Completed",
                Data = new ProgressData { Percent = 100, Message = "章節新增完成" },
                clientRequestId = dto.clientRequestId
            });

            return addedChapter.ChapterId;
        }

        public async Task<bool> UpdateChapterAsync(int id, VChapterDTO dto, string ConnectionId, IHubContext<CourseHub> hubContext)
        {
            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Chapter:Started",
                Data = new ProgressData { Percent = 10, Message = "開始更新章節" },
                clientRequestId = dto.clientRequestId
            });

            var chapter = await _unitOfWork.Chapters.GetByIdAsync(id);
            if (chapter == null)
                throw new ArgumentException("找不到指定章節", nameof(id));

            chapter.ChapterTitle = dto.ChapterTitle;
            chapter.ChapterDes = dto.ChapterDes;

            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Chapter:SavingToDb",
                Data = new ProgressData { Percent = 50, Message = "正在更新資料庫..." },
                clientRequestId = dto.clientRequestId
            });

            _unitOfWork.Chapters.Update(chapter);
            await _unitOfWork.CompleteAsync();

            await hubContext.Clients.Client(ConnectionId).SendAsync("ReceiveProgress", new ProgressUpdate
            {
                Step = "Chapter:Completed",
                Data = new ProgressData { Percent = 100, Message = "章節更新完成" },
                clientRequestId = dto.clientRequestId
            });

            return true;
        }

        public async Task<TCourseChapter> GetChapterAsync(int id)
        {
            return await _unitOfWork.Chapters.GetByIdAsync(id);
        }

        public async Task<IEnumerable<TCourseChapter>> GetAllChaptersAsync()
        {
            return await _unitOfWork.Chapters.GetAllAsync();
        }

        public async Task<bool> DeleteChapterAsync(int id)
        {
            var chapter = await _unitOfWork.Chapters.GetByIdAsync(id);
            if (chapter == null)
                return false;

            _unitOfWork.Chapters.Delete(chapter);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
