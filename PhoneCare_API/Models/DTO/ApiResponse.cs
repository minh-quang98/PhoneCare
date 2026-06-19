namespace PhoneCare_API.Models.DTO
{
    public class ApiResponse<TData>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public TData? Data { get; set; }
        public object? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public static ApiResponse<TData> Create(bool success, int statusCode, string? message, TData? data = default, object? error = null)
        {
            return new ApiResponse<TData>
            {
                Success = success,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = error,
            };
        }

        public static ApiResponse<TData> Ok(string message, TData? data = default)
        {
            return Create(true, StatusCodes.Status200OK, message, data);
        }

        public static ApiResponse<TData> Created(string message, TData? data = default)
        {
            return Create(true, StatusCodes.Status201Created, message, data);
        }

        public static ApiResponse<TData> BadRequest(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status400BadRequest, message, default, error);
        }

        public static ApiResponse<TData> Unauthorized(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status401Unauthorized, message, default, error);
        }

        public static ApiResponse<TData> Forbidden(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status403Forbidden, message, default, error);
        }

        public static ApiResponse<TData> NotFound(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status404NotFound, message, default, error);
        }

        public static ApiResponse<TData> Conflict(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status409Conflict, message, default, error);
        }

        public static ApiResponse<TData> InternalServerError(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status500InternalServerError, message, default, error);
        }
    }
}
