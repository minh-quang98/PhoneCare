using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneCare.Models;
using PhoneCare_API.Data;
using PhoneCare_API.Models.DTO;
using PhoneCare_API.Services;

namespace PhoneCare_API.Controllers
{
    [Route("api/nhan-vien")]
    [ApiController]
    public class NhanVienController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly CurrentUserService _currentUserService;

        public NhanVienController(ApplicationDbContext db, CurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<NhanVienListItemDto>>>> GetAll()
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<IEnumerable<NhanVienListItemDto>>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanManageEmployees(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<IEnumerable<NhanVienListItemDto>>.Forbidden("Bạn không có quyền quản lý nhân viên."));

            var data = await _db.NhanViens
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.FullName)
                .Select(x => new NhanVienListItemDto
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Email = x.Email,
                    Phone = x.Phone,
                    LoaiNhanVien = x.LoaiNhanVien,
                    IdCoSoLamViec = x.IdCoSoLamViec,
                    WorkPlaceName = x.CoSoCuaHang != null ? x.CoSoCuaHang.Name ?? string.Empty : string.Empty,
                    KhoaTaiKhoan = x.KhoaTaiKhoan
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<NhanVienListItemDto>>.Ok("Lấy danh sách nhân viên thành công.", data));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<NhanVienDetailDto>>> GetById(int id)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<NhanVienDetailDto>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanManageEmployees(current.LoaiNhanVien) && current.Id != id)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<NhanVienDetailDto>.Forbidden("Bạn không có quyền xem nhân viên này."));
            }

            var item = await _db.NhanViens.Include(x => x.CoSoCuaHang).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<NhanVienDetailDto>.NotFound("Không tìm thấy nhân viên."));
            return Ok(ApiResponse<NhanVienDetailDto>.Ok("Lấy nhân viên thành công.", MapDetail(item)));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<NhanVienDetailDto>>> Create(CreateNhanVienDto request)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<NhanVienDetailDto>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanManageEmployees(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<NhanVienDetailDto>.Forbidden("Bạn không có quyền quản lý nhân viên."));

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(ApiResponse<NhanVienDetailDto>.BadRequest("Mật khẩu không được để trống."));
            }

            var validation = ValidateEmployee(request.UserName, request.FullName, request.LoaiNhanVien);
            if (validation != null) return BadRequest(ApiResponse<NhanVienDetailDto>.BadRequest(validation));
            if (await _db.NhanViens.AnyAsync(x => x.UserName == request.UserName.Trim() && !x.IsDeleted))
            {
                return Conflict(ApiResponse<NhanVienDetailDto>.Conflict($"Tài khoản {request.UserName} đã tồn tại."));
            }
            if (!await _db.CoSoCuaHangs.AnyAsync(x => x.Id == request.IdCoSoLamViec && !x.IsDeleted))
            {
                return BadRequest(ApiResponse<NhanVienDetailDto>.BadRequest(
                    "Không tồn tại cơ sở cửa hàng đã chọn hoặc cơ sở này đã bị xóa."));
            }

            var item = new NhanVien
            {
                UserName = request.UserName.Trim(),
                Password = PasswordHasher.Hash(request.Password.Trim()),
                FullName = request.FullName.Trim(),
                NickName = request.NickName?.Trim(),
                Email = request.Email?.Trim(),
                Phone = request.Phone?.Trim(),
                IdCoSoLamViec = request.IdCoSoLamViec,
                KhoaTaiKhoan = request.KhoaTaiKhoan,
                LoaiNhanVien = request.LoaiNhanVien.Trim(),
                DateCreated = DateTime.Now,
                UserCreated = current.Id,
                IsDeleted = false
            };
            _db.NhanViens.Add(item);
            await _db.SaveChangesAsync();
            await _db.Entry(item).Reference(x => x.CoSoCuaHang).LoadAsync();

            return StatusCode(StatusCodes.Status201Created, ApiResponse<NhanVienDetailDto>.Created("Thêm nhân viên thành công.", MapDetail(item)));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<NhanVienDetailDto>>> Update(int id, UpdateNhanVienDto request)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<NhanVienDetailDto>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanManageEmployees(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<NhanVienDetailDto>.Forbidden("Bạn không có quyền quản lý nhân viên."));

            var item = await _db.NhanViens.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<NhanVienDetailDto>.NotFound("Không tìm thấy nhân viên."));

            var validation = ValidateEmployee(request.UserName, request.FullName, request.LoaiNhanVien);
            if (validation != null) return BadRequest(ApiResponse<NhanVienDetailDto>.BadRequest(validation));
            if (await _db.NhanViens.AnyAsync(x => x.Id != id && x.UserName == request.UserName.Trim() && !x.IsDeleted))
            {
                return Conflict(ApiResponse<NhanVienDetailDto>.Conflict($"Tài khoản {request.UserName} đã tồn tại."));
            }
            if (!await _db.CoSoCuaHangs.AnyAsync(x => x.Id == request.IdCoSoLamViec && !x.IsDeleted))
            {
                return BadRequest(ApiResponse<NhanVienDetailDto>.BadRequest(
                    "Không tồn tại cơ sở cửa hàng đã chọn hoặc cơ sở này đã bị xóa."));
            }

            item.UserName = request.UserName.Trim();
            item.FullName = request.FullName.Trim();
            item.NickName = request.NickName?.Trim();
            item.Email = request.Email?.Trim();
            item.Phone = request.Phone?.Trim();
            item.IdCoSoLamViec = request.IdCoSoLamViec;
            item.KhoaTaiKhoan = request.KhoaTaiKhoan;
            item.LoaiNhanVien = request.LoaiNhanVien.Trim();
            item.DateModify = DateTime.Now;
            item.UserModify = current.Id;
            await _db.SaveChangesAsync();
            await _db.Entry(item).Reference(x => x.CoSoCuaHang).LoadAsync();

            return Ok(ApiResponse<NhanVienDetailDto>.Ok("Cập nhật nhân viên thành công.", MapDetail(item)));
        }

        [HttpPatch("{id:int}/lock")]
        public async Task<ActionResult<ApiResponse<object>>> SetLock(int id, SetLockNhanVienDto request)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<object>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanManageEmployees(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.Forbidden("Bạn không có quyền quản lý nhân viên."));

            var item = await _db.NhanViens.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<object>.NotFound("Không tìm thấy nhân viên."));

            item.KhoaTaiKhoan = request.KhoaTaiKhoan;
            item.DateModify = DateTime.Now;
            item.UserModify = current.Id;
            await _db.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok(request.KhoaTaiKhoan ? "Khóa tài khoản thành công." : "Mở khóa tài khoản thành công."));
        }

        [HttpPatch("{id:int}/password")]
        public async Task<ActionResult<ApiResponse<object>>> ResetPassword(int id, ResetPasswordDto request)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<object>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanManageEmployees(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.Forbidden("Bạn không có quyền quản lý nhân viên."));
            if (string.IsNullOrWhiteSpace(request.Password)) return BadRequest(ApiResponse<object>.BadRequest("Mật khẩu không được để trống."));

            var item = await _db.NhanViens.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<object>.NotFound("Không tìm thấy nhân viên."));

            item.Password = PasswordHasher.Hash(request.Password.Trim());
            item.FailedLoginCount = 0;
            item.LockoutEndAt = null;
            item.LastFailedLoginAt = null;
            item.DateModify = DateTime.Now;
            item.UserModify = current.Id;
            await _db.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok("Đổi mật khẩu nhân viên thành công."));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<object>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanManageEmployees(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.Forbidden("Bạn không có quyền quản lý nhân viên."));

            var item = await _db.NhanViens.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<object>.NotFound("Không tìm thấy nhân viên."));

            item.IsDeleted = true;
            item.DateModify = DateTime.Now;
            item.UserModify = current.Id;
            await _db.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok("Xóa nhân viên thành công."));
        }

        private static string? ValidateEmployee(string userName, string fullName, string role)
        {
            if (string.IsNullOrWhiteSpace(userName)) return "Tài khoản không được để trống.";
            if (string.IsNullOrWhiteSpace(fullName)) return "Họ và tên không được để trống.";
            if (!PermissionService.Roles.Any(x => string.Equals(x, role, StringComparison.OrdinalIgnoreCase))) return "Loại nhân viên không hợp lệ.";
            return null;
        }

        private static NhanVienDetailDto MapDetail(NhanVien item)
        {
            return new NhanVienDetailDto
            {
                Id = item.Id,
                UserName = item.UserName,
                FullName = item.FullName,
                NickName = item.NickName,
                Email = item.Email,
                Phone = item.Phone,
                LoaiNhanVien = item.LoaiNhanVien,
                IdCoSoLamViec = item.IdCoSoLamViec,
                WorkPlaceName = item.CoSoCuaHang?.Name ?? string.Empty,
                KhoaTaiKhoan = item.KhoaTaiKhoan,
                FailedLoginCount = item.FailedLoginCount,
                LockoutEndAt = item.LockoutEndAt,
                LastFailedLoginAt = item.LastFailedLoginAt
            };
        }
    }
}
