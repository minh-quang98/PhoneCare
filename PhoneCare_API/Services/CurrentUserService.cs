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

        public CurrentUserService(AuthTokenService tokenService, ApplicationDbContext db)
        {
            _tokenService = tokenService;
            _db = db;
        }

        public async Task<NhanVien?> GetCurrentNhanVienAsync(HttpContext httpContext)
        {
            var user = GetCurrentUser(httpContext);
            if (user == null) return null;

            return await _db.NhanViens
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == user.Id && !x.IsDeleted && !x.KhoaTaiKhoan);
        }

        public CurrentUserDto? GetCurrentUser(HttpContext httpContext)
        {
            var header = httpContext.Request.Headers.Authorization.ToString();
            const string prefix = "Bearer ";
            if (!header.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return null;

            return _tokenService.TryValidate(header[prefix.Length..].Trim(), out var user) ? user : null;
        }
    }
}
