using Microsoft.Data.SqlClient;
using PhoneCare_API.Models.DTO;

namespace PhoneCare_API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        /// <summary>
        /// Khởi tạo đối tượng GlobalExceptionMiddleware.
        /// </summary>
        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Bao bọc pipeline HTTP để xử lý tập trung các ngoại lệ chưa được bắt.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // Client đã chủ động đóng kết nối, không phải lỗi của API.
                _logger.LogInformation(
                    "Request {Method} {Path} was cancelled by the client. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);
            }
            catch (SqlException exception) when (IsSqlConnectionFailure(exception))
            {
                await HandleSqlConnectionExceptionAsync(context, exception);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        /// <summary>
        /// Ghi log và trả về phản hồi khi không thể kết nối SQL Server.
        /// </summary>
        private async Task HandleSqlConnectionExceptionAsync(HttpContext context, SqlException exception)
        {
            _logger.LogError(
                exception,
                "Could not connect to SQL Server while processing {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.ContentType = "application/json; charset=utf-8";

            var response = ApiResponse<object>.ServiceUnavailable(
                "Không thể kết nối đến cơ sở dữ liệu. Vui lòng thử lại sau.",
                new { traceId = context.TraceIdentifier });

            await context.Response.WriteAsJsonAsync(response);
        }

        /// <summary>
        /// Xác định SqlException có thuộc nhóm lỗi kết nối hoặc lỗi mạng hay không.
        /// </summary>
        private static bool IsSqlConnectionFailure(SqlException exception)
        {
            // Common SQL Server and network error numbers raised when the server
            // cannot be reached or an established connection is interrupted.
            int[] connectionErrorNumbers =
            [
                -1, 2, 20, 53, 64, 233, 258,
                10053, 10054, 10060, 11001
            ];

            return exception.Errors
                .Cast<SqlError>()
                .Any(error => connectionErrorNumbers.Contains(error.Number));
        }

        /// <summary>
        /// Ghi log và trả về phản hồi lỗi nội bộ thống nhất cho client.
        /// </summary>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            if (context.Response.HasStarted)
            {
                // Không thể thay đổi response khi header/body đã được gửi về client.
                return;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json; charset=utf-8";

            var response = ApiResponse<object>.InternalServerError(
                "Đã xảy ra lỗi trong quá trình xử lý. Vui lòng thử lại sau.",
                new { traceId = context.TraceIdentifier });

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
