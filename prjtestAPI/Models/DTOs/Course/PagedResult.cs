namespace prjEvolutionAPI.Models.DTOs.Course
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = null!;
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
