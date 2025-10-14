namespace ESTop1.Api.DTOs;

public class ErrorResponse
{
    public string Message { get; set; } = null!;
    public string? Details { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ValidationErrorResponse : ErrorResponse
{
    public Dictionary<string, string[]> Errors { get; set; } = new();
}
