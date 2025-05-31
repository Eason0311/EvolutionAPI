public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int? StatusCode { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> SuccessResponse(string? message = null, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> FailResponse(string message, List<string>? errors = null, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> FailResponse(List<string> errors, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Errors = errors,
            StatusCode = statusCode
        };
    }
}
