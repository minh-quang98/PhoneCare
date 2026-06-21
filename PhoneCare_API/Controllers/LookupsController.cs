using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneCare.Models;
using PhoneCare_API.Data;
using PhoneCare_API.Models.DTO;
using PhoneCare_API.Services;

namespace PhoneCare_API.Controllers
{
    [Route("api/lookups")]
    [ApiController]
    public class LookupsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        /// <summary>
        /// Khởi tạo controller cung cấp dữ liệu danh mục dùng chung.
        /// </summary>
        public LookupsController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Trả về danh sách vai trò nhân viên được hỗ trợ.
        /// </summary>
        [HttpGet("roles")]
        public ActionResult<ApiResponse<IEnumerable<LookupItemDto>>> Roles()
        {
            var data = PermissionService.Roles
                .Select((role, index) => new LookupItemDto { Id = index + 1, Text = role })
                .ToList();

            return Ok(ApiResponse<IEnumerable<LookupItemDto>>.Ok("Lấy danh sách vai trò thành công.", data));
        }

        /// <summary>
        /// Trả về danh sách trạng thái sửa chữa dùng cho lookup.
        /// </summary>
        [HttpGet("repair-statuses")]
        public ActionResult<ApiResponse<IEnumerable<LookupItemDto>>> RepairStatuses()
        {
            var data = Enum.GetValues<RepairStatus>()
                .Select(x => new LookupItemDto { Id = (int)x, Text = RepairStatusService.GetText((int)x) })
                .ToList();
            return Ok(ApiResponse<IEnumerable<LookupItemDto>>.Ok("Lấy danh sách trạng thái sửa chữa thành công.", data));
        }

        /// <summary>
        /// Lấy danh sách kỹ thuật viên đang hoạt động.
        /// </summary>
        [HttpGet("technicians")]
        public async Task<ActionResult<ApiResponse<IEnumerable<NhanVienListItemDto>>>> Technicians()
        {
            var data = await _db.NhanViens
                .Include(x => x.CoSoCuaHang)
                .Where(x => !x.IsDeleted && !x.KhoaTaiKhoan && x.LoaiNhanVien == PermissionService.KyThuat)
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

            return Ok(ApiResponse<IEnumerable<NhanVienListItemDto>>.Ok("Lấy danh sách kỹ thuật viên thành công.", data));
        }
    }
}
