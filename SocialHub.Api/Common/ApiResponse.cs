using System.Text.Json.Serialization;

namespace SocialHub.Api.Common;

public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> Ok(T data, string message = "Operation completed successfully.")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = new List<string>()
        };
    }

    public static ApiResponse<T> Fail(string message, List<string>? errors = null, T? data = default)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = data,
            Errors = errors ?? new List<string>()
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(string message = "Operation completed successfully.")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Data = null,
            Errors = new List<string>()
        };
    }

    public static ApiResponse Fail(string message, List<string>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Data = null,
            Errors = errors ?? new List<string>()
        };
    }
}
