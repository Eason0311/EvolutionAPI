using System.Text.Json.Serialization;

namespace prjEvolutionAPI.Responses
{
    public class LinePayRequestInfo
    {
        [JsonPropertyName("transactionId")]
        public long TransactionId { get; set; }

        [JsonPropertyName("paymentUrl")]
        public PaymentUrl PaymentUrl { get; set; } = null!;
    }
}
