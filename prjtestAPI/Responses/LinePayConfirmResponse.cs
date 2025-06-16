using System.Text.Json.Serialization;

namespace prjEvolutionAPI.Responses
{
    public class LinePayConfirmResponse
    {
        [JsonPropertyName("returnCode")]
        public string ReturnCode { get; set; } = null!;

        [JsonPropertyName("returnMessage")]
        public string ReturnMessage { get; set; } = null!;

        [JsonPropertyName("info")]
        public LinePayConfirmInfo Info { get; set; } = null!;
    }
}
