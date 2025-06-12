using System.Text.Json.Serialization;

namespace prjEvolutionAPI.Responses
{
    public class LinePayRequestResponse
    {
        [JsonPropertyName("returnCode")]
        public string ReturnCode { get; set; } = null!;

        [JsonPropertyName("returnMessage")]
        public string ReturnMessage { get; set; } = null!;

        // 這裡告訴反序列器：JSON key "info" 要對應到這個屬性
        [JsonPropertyName("info")]
        public LinePayRequestInfo Info { get; set; } = null!;
    }
}
