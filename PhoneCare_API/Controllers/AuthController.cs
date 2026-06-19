using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneCare_API.Data;
using PhoneCare_API.Models.DTO;
using PhoneCare_API.Services;

namespace PhoneCare_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);
        private readonly ApplicationDbContext _db;
        private readonly AuthTokenService _tokenService;
        private readonly CurrentUserService _currentUserService;

        /// <summary>
        /// Khởi tạo controller xác thực cùng các dịch vụ phụ thuộc.
        /// </summary>
        public AuthController(ApplicationDbContext db, AuthTokenService tokenService, CurrentUserService currentUserService)
        {
            _db = db;
            _tokenService = tokenService;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Xác thực tài khoản, quản lý số lần đăng nhập sai và cấp token đăng nhập.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(ApiResponse<LoginResponseDto>.BadRequest("Tài khoản và mật khẩu không được để trống."));
            }

            var now = DateTime.Now;
            var user = await _db.NhanViens.FirstOrDefaultAsync(x => x.UserName == request.UserName.Trim() && !x.IsDeleted);
            if (user != null && user.LockoutEndAt.HasValue && user.LockoutEndAt.Value > now)
            {
                return StatusCode(StatusCodes.Status423Locked, ApiResponse<LoginResponseDto>.Create(false, StatusCodes.Status423Locked, $"Tài khoản đang bị khóa tạm thời đến {user.LockoutEndAt.Value:HH:mm:ss}."));
            }

            if (user != null && !user.KhoaTaiKhoan && IsPasswordValid(user.Password, request.Password))
            {
                if (!PasswordHasher.IsHashed(user.Password))
                {
                    user.Password = PasswordHasher.Hash(request.Password);
                }

                user.FailedLoginCount = 0;
                user.LockoutEndAt = null;
                user.LastFailedLoginAt = null;
                await _db.SaveChangesAsync();

                var currentUser = new CurrentUserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    CoSoCuaHangId = user.IdCoSoLamViec,
                    LoaiNhanVien = user.LoaiNhanVien
                };
                var token = _tokenService.CreateToken(currentUser);

                return Ok(ApiResponse<LoginResponseDto>.Ok("Đăng nhập thành công.", new LoginResponseDto
                {
                    Token = token.Token,
                    ExpiresAt = token.ExpiresAt,
                    User = currentUser
                }));
            }

            if (user != null && !user.KhoaTaiKhoan)
            {
                user.FailedLoginCount += 1;
                user.LastFailedLoginAt = now;
                if (user.FailedLoginCount >= MaxFailedAttempts)
                {
                    user.LockoutEndAt = now.Add(LockoutDuration);
                }

                await _db.SaveChangesAsync();
            }

            return Unauthorized(ApiResponse<LoginResponseDto>.Unauthorized("Sai tài khoản hoặc mật khẩu."));
        }

        /// <summary>
        /// Lấy thông tin người dùng hiện đang đăng nhập từ token.
        /// </summary>
        [HttpGet("current-user")]
        public async Task<ActionResult<ApiResponse<CurrentUserDto>>> CurrentUser()
        {
            var user = await _currentUserService.GetCurrentNhanVienAsync(HttpContext);
            if (user == null)
            {
                return Unauthorized(ApiResponse<CurrentUserDto>.Unauthorized("Token không hợp lệ hoặc tài khoản không còn hoạt động."));
            }

            return Ok(ApiResponse<CurrentUserDto>.Ok("Lấy thông tin người dùng hiện tại thành công.", new CurrentUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                CoSoCuaHangId = user.IdCoSoLamViec,
                LoaiNhanVien = user.LoaiNhanVien
            }));
        }

        /// <summary>
        /// Kiểm tra mật khẩu hiện tại và cập nhật mật khẩu mới cho người dùng.
        /// </summary>
        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponse<object>>> ChangePassword(ChangePasswordRequestDto request)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<object>.Unauthorized("Vui lòng đăng nhập."));

            if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(ApiResponse<object>.BadRequest("Mật khẩu cũ và mật khẩu mới không được để trống."));
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return BadRequest(ApiResponse<object>.BadRequest("Mật khẩu nhập lại không khớp."));
            }

            var user = await _db.NhanViens.FirstOrDefaultAsync(x => x.Id == current.Id && !x.IsDeleted);
            if (user == null) return NotFound(ApiResponse<object>.NotFound("Không tìm thấy tài khoản."));
            if (!IsPasswordValid(user.Password, request.OldPassword))
            {
                return BadRequest(ApiResponse<object>.BadRequest("Mật khẩu cũ không đúng."));
            }

            user.Password = PasswordHasher.Hash(request.NewPassword);
            user.DateModify = DateTime.Now;
            user.UserModify = current.Id;
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok("Đổi mật khẩu thành công."));
        }

        /// <summary>
        /// Xác thực mật khẩu nhập vào và hỗ trợ dữ liệu mật khẩu cũ chưa được băm.
        /// </summary>
        private static bool IsPasswordValid(string storedPassword, string password)
        {
            return PasswordHasher.IsHashed(storedPassword)
                ? PasswordHasher.Verify(password, storedPassword)
                : storedPassword == password;
        }
    }
}
