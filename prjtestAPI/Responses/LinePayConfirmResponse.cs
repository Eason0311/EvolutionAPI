namespace prjEvolutionAPI.Responses
{
    public class LinePayConfirmResponse
    {
        public string ReturnCode { get; set; } = null!;
        public string ReturnMessage { get; set; } = null!;
        public LinePayConfirmInfo Info { get; set; } = null!;
    }
}
