using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneCare_API.Data;
using PhoneCare_API.Models.DTO;
using PhoneCare_API.Services;

namespace PhoneCare_API.Controllers
{
    [Route("api/dich-vu")]
    [ApiController]
    public class DichVuController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly CurrentUserService _currentUserService;

        /// <summary>
        /// Khởi tạo controller quản lý dịch vụ cùng các dịch vụ phụ thuộc.
        /// </summary>
        public DichVuController(ApplicationDbContext db, CurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Kiểm tra dữ liệu và cập nhật bản ghi được yêu cầu.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<DichVuDto>>> Update(int id, UpdateDichVuDto request)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<DichVuDto>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanManageServices(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<DichVuDto>.Forbidden("Bạn không có quyền cập nhật dịch vụ."));

            var item = await _db.DichVus.Include(x => x.DonHang).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<DichVuDto>.NotFound("Không tìm thấy dịch vụ."));
            if (item.DonHang == null || item.DonHang.IsDeleted) return NotFound(ApiResponse<DichVuDto>.NotFound("Không tìm thấy đơn hàng."));
            if (!RepairStatusService.CanEditOrder(item.DonHang.TinhTrang)) return BadRequest(ApiResponse<DichVuDto>.BadRequest("Không thể cập nhật dịch vụ ở trạng thái hiện tại."));

            var validation = DonHangController.ValidateService(request.TenDichVu, request.DonGia);
            if (validation != null) return BadRequest(ApiResponse<DichVuDto>.BadRequest(validation));

            item.TenDichVu = request.TenDichVu.Trim();
            item.DonGia = request.DonGia;
            item.DateModify = DateTime.Now;
            item.UserModify = current.Id;
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<DichVuDto>.Ok("Cập nhật dịch vụ thành công.", DonHangController.MapService(item)));
        }

        /// <summary>
        /// Kiểm tra quyền và thực hiện xóa mềm bản ghi được yêu cầu.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<object>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanManageServices(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.Forbidden("Bạn không có quyền cập nhật dịch vụ."));

            var item = await _db.DichVus.Include(x => x.DonHang).FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<object>.NotFound("Không tìm thấy dịch vụ."));
            if (item.DonHang == null || item.DonHang.IsDeleted) return NotFound(ApiResponse<object>.NotFound("Không tìm thấy đơn hàng."));
            if (!RepairStatusService.CanEditOrder(item.DonHang.TinhTrang)) return BadRequest(ApiResponse<object>.BadRequest("Không thể cập nhật dịch vụ ở trạng thái hiện tại."));

            item.IsDeleted = true;
            item.DateModify = DateTime.Now;
            item.UserModify = current.Id;
            await _db.SaveChangesAsync();

            return Ok(ApiResponse<object>.Ok("Xóa dịch vụ thành công."));
        }
    }
}
