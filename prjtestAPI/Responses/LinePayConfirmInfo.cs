namespace prjEvolutionAPI.Responses
{
    public class LinePayConfirmInfo
    {
        public long TransactionId { get; set; }
        public string OrderId { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
