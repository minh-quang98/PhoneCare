using Microsoft.EntityFrameworkCore;
using PhoneCare.Models;
using PhoneCare_API.Data;
using PhoneCare_API.Models.DTO;

namespace PhoneCare_API.Services
{
    public class CurrentUserService
    {
        private readonly AuthTokenService _tokenService;
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Khởi tạo dịch vụ truy xuất người dùng hiện tại.
        /// </summary>
        public CurrentUserService(AuthTokenService tokenService, ApplicationDbContext db)
        {
            _tokenService = tokenService;
            _db = db;
        }

        /// <summary>
        /// Xác thực token và tải nhân viên hiện tại từ cơ sở dữ liệu.
        /// </summary>
        public async Task<NhanVien?> GetCurrentNhanVienAsync(HttpContext httpContext)
        {
            var user = GetCurrentUser(httpContext);
            if (user == null) return null;

            return await _db.NhanViens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == user.Id && !x.IsDeleted && !x.KhoaTaiKhoan);
        }

        /// <summary>
        /// Đọc và xác thực token để lấy thông tin người dùng hiện tại.
        /// </summary>
        public CurrentUserDto? GetCurrentUser(HttpContext httpContext)
        {
            var header = httpContext.Request.Headers.Authorization.ToString().Trim();
            const string prefix = "Bearer ";
            if (string.IsNullOrWhiteSpace(header)) return null;

            // Scalar/Postman tự thêm tiền tố Bearer, trong khi một số client gửi trực tiếp
            // token vào Authorization. Chấp nhận cả hai dạng để tránh từ chối token hợp lệ.
            while (header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                header = header[prefix.Length..].Trim();
            }

            return _tokenService.TryValidate(header, out var user) ? user : null;
        }
    }
}
