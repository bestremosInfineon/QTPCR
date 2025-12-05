namespace QTPCR.Models
{
    public class ErrorModel
    {
        public string? FunctionName { get; set; }
        public string? Message { get; set; }
        public string? Parameters { get; set; }
        public string? ExceptionStackTrace { get; set; }
    }
}
