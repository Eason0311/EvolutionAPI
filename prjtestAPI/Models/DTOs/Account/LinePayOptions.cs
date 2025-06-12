namespace prjEvolutionAPI.Models.DTOs.Account
{
    public class LinePayOptions
    {
        public string ChannelId { get; set; } = null!;
        public string ChannelSecret { get; set; } = null!;
        public string Endpoint { get; set; } = null!;
        public string ConfirmUrl { get; set; } = null!;
        public string CancelUrl { get; set; } = null!;
    }
}
