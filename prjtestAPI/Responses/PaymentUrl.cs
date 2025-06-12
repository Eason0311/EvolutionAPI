using System.Text.Json.Serialization;

namespace prjEvolutionAPI.Responses
{
    public class PaymentUrl
    {
        [JsonPropertyName("web")]
        public string Web { get; set; } = null!;

        [JsonPropertyName("app")]
        public string App { get; set; } = null!;
    }
}
