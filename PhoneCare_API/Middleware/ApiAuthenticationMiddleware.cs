using PhoneCare_API.Models.DTO;
using PhoneCare_API.Services;

namespace PhoneCare_API.Middleware
{
    public class ApiAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, CurrentUserService currentUserService)
        {
            if (!context.Request.Path.StartsWithSegments("/api") || IsLoginRequest(context))
            {
                await _next(context);
                return;
            }

            if (currentUserService.GetCurrentUser(context) == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.Unauthorized("Token không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại."));
                return;
            }

            await _next(context);
        }

        private static bool IsLoginRequest(HttpContext context)
        {
            return HttpMethods.IsPost(context.Request.Method)
                && string.Equals(context.Request.Path.Value, "/api/auth/login", StringComparison.OrdinalIgnoreCase);
        }
    }
}
