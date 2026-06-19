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

        /// <summary>
        /// Tạo phản hồi API thống nhất từ trạng thái, mã HTTP, dữ liệu và thông tin lỗi.
        /// </summary>
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

        /// <summary>
        /// Tạo phản hồi API cho thao tác thành công.
        /// </summary>
        public static ApiResponse<TData> Ok(string message, TData? data = default)
        {
            return Create(true, StatusCodes.Status200OK, message, data);
        }

        /// <summary>
        /// Tạo phản hồi API cho tài nguyên vừa được tạo thành công.
        /// </summary>
        public static ApiResponse<TData> Created(string message, TData? data = default)
        {
            return Create(true, StatusCodes.Status201Created, message, data);
        }

        /// <summary>
        /// Tạo phản hồi API cho yêu cầu có dữ liệu không hợp lệ.
        /// </summary>
        public static ApiResponse<TData> BadRequest(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status400BadRequest, message, default, error);
        }

        /// <summary>
        /// Tạo phản hồi API khi người dùng chưa được xác thực.
        /// </summary>
        public static ApiResponse<TData> Unauthorized(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status401Unauthorized, message, default, error);
        }

        /// <summary>
        /// Tạo phản hồi API khi người dùng không có quyền thực hiện thao tác.
        /// </summary>
        public static ApiResponse<TData> Forbidden(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status403Forbidden, message, default, error);
        }

        /// <summary>
        /// Tạo phản hồi API khi không tìm thấy tài nguyên được yêu cầu.
        /// </summary>
        public static ApiResponse<TData> NotFound(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status404NotFound, message, default, error);
        }

        /// <summary>
        /// Tạo phản hồi API khi dữ liệu yêu cầu xung đột với trạng thái hiện tại.
        /// </summary>
        public static ApiResponse<TData> Conflict(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status409Conflict, message, default, error);
        }

        /// <summary>
        /// Tạo phản hồi API cho lỗi nội bộ máy chủ.
        /// </summary>
        public static ApiResponse<TData> InternalServerError(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status500InternalServerError, message, default, error);
        }

        /// <summary>
        /// Tạo phản hồi API khi dịch vụ tạm thời không khả dụng.
        /// </summary>
        public static ApiResponse<TData> ServiceUnavailable(string message, object? error = null)
        {
            return Create(false, StatusCodes.Status503ServiceUnavailable, message, default, error);
        }
    }
}
