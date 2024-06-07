namespace SigmaAssessment.Helpers
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public object Data { get; set; }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object? Errors { get; set; }
    }
}
