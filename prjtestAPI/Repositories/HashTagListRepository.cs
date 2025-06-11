using Microsoft.EntityFrameworkCore;
using prjEvolutionAPI.Models;
using prjEvolutionAPI.Models.DTOs.Course;
using prjEvolutionAPI.Models.DTOs.CreateCourse;
using prjEvolutionAPI.Repositories.Interfaces;

namespace prjEvolutionAPI.Repositories
{
    public class HashTagListRepository : IHashTagListRepository
    {
        private readonly EvolutionApiContext _context;
        public HashTagListRepository(EvolutionApiContext context)
        {
            _context = context;
        }

        public async Task<List<HashTagListDTO>> GetRandomTagAsync(int count = 3)
        {
            return await _context.THashTagLists
                                 .OrderBy(c => Guid.NewGuid())
                                 .Take(count)
                                 .Select(c => new HashTagListDTO
                                 {
                                     TagId = c.TagId,
                                     TagName = c.TagName
                                 })
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ResHashTagDTO>> GetAllHashTagsAsync()
        {
            var hashTags = await _context.THashTagLists
                .Select(n => new ResHashTagDTO
                {
                    HashTagId = n.TagId,
                    HashTagName = n.TagName
                })
                .ToListAsync();

            return hashTags;
        }
    }
}
