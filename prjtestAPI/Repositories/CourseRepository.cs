using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Course;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly EvolutionApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public CourseRepository(IConfiguration configuration, EvolutionApiContext context)
        {
            _context = context;
            _configuration = configuration;
            _baseUrl = _configuration.GetValue<string>("AppSettings:BaseUrl")?.TrimEnd('/')
                ?? throw new InvalidOperationException("AppSettings:BaseUrl 未設定");
        }

        public async Task AddAsync(TCourse entity)
        {
            await _context.TCourses.AddAsync(entity);
        }

        public void Delete(TCourse entity)
        {
            _context.TCourses.Remove(entity);
        }

        public async Task<bool> ExistsAsync(int courseId)
        {
            return await _context.TCourses
                .AsNoTracking()
                .AnyAsync(c => c.CourseId == courseId);
        }
        public async Task<IEnumerable<CourseDTO>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.TCourses
        .AsNoTracking()
        .Include(c => c.Company)
        .Where(c => ids.Contains(c.CourseId))
        // 這裡加上 Select 投影到你的 DTO
        .Select(c => new CourseDTO
        {
            CourseId = c.CourseId,
            CompanyId = c.CompanyId,
            CompanyName = c.Company.CompanyName,
            CourseTitle = c.CourseTitle,
            CourseDes = c.CourseDes,
            IsPublic = c.IsPublic,
            // 如果你在 service／repo 裡面有 _baseUrl 變數
            CoverImagePath = $"{_baseUrl}/{c.CoverImagePath.TrimStart('/')}",
            Price = c.Price
        })
        .ToListAsync();
        }

        public async Task<IEnumerable<TCourse>> GetAllAsync()
        {
            return await _context.TCourses
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CourseDTO?> GetByIdAsync(int courseId)
        {
            var item = await _context.TCourses
                .AsNoTracking()
                .Include(c => c.Company)
                .Where(c => c.CourseId == courseId)
                .Select(c => new CourseDTO
                {
                    CourseId = c.CourseId,
                    CompanyId = c.CompanyId,
                    CompanyName = c.Company.CompanyName,
                    CourseTitle = c.CourseTitle,
                    CourseDes = c.CourseDes,
                    IsPublic = c.IsPublic,
                    // 同樣把 baseUrl 加進去
                    CoverImagePath = $"{_baseUrl}/{c.CoverImagePath.TrimStart('/')}",
                    Price = c.Price
                }).FirstOrDefaultAsync();

            return item;
        }

        public async Task<IEnumerable<TCourse>> GetByCompanyIdAsync(int companyId)
        {
            return await _context.TCourses
                .AsNoTracking()
                .Where(c => c.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<(IEnumerable<CourseDTO> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize)
        {
            var totalCount = await _context.TCourses.CountAsync();

            var items = await _context.TCourses
                .AsNoTracking()
                .Include(c => c.Company)
                .OrderBy(c => c.CourseId)               // ← 加上這行，保證順序一致
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(c => new CourseDTO
                {
                    CourseId = c.CourseId,
                    CompanyId = c.CompanyId,
                    CompanyName = c.Company.CompanyName,
                    CourseTitle = c.CourseTitle,
                    CourseDes = c.CourseDes,
                    IsPublic = c.IsPublic,
                    CoverImagePath = $"{_baseUrl}/{c.CoverImagePath.TrimStart('/')}",
                    Price = c.Price
                })
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<TCourse>> SearchAsync(string keyword)
        {
            return await _context.TCourses
                .AsNoTracking()
                .Where(c => EF.Functions.Like(c.CourseTitle, $"%{keyword}%")
                         || EF.Functions.Like(c.CourseDes ?? string.Empty, $"%{keyword}%"))
                .ToListAsync();
        }

        public void Update(TCourse entity)
        {
            _context.TCourses.Update(entity);
        }
    }
}
