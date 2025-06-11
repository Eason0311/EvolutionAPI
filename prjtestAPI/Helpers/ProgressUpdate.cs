namespace prjEvolutionAPI.Helpers
{
    public class ProgressUpdate
    {
        public string Step { get; set; }
        public ProgressData Data { get; set; }
        public string clientRequestId { get; set; } = null!; // 用於追蹤請求的唯一識別碼
    }

    public class ProgressData
    {
        public int Percent { get; set; }
        public string Message { get; set; }
    }
}
