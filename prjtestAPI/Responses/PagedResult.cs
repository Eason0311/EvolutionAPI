namespace prjEvolutionAPI.Responses
{
    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int Total { get; set; }
    }
}
