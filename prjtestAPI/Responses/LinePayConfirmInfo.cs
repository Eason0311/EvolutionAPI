using System.Text.Json.Serialization;

namespace prjEvolutionAPI.Responses
{
    public class LinePayConfirmInfo
    {
        [JsonPropertyName("transactionId")]
        public long TransactionId { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; } = null!;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = null!;
    }
}
