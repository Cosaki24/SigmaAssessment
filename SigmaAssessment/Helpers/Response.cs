using SigmaAssessment.Models;

namespace SigmaAssessment.Helpers
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public Candidate? Data { get; set; }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }
}
