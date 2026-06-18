using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneCare.Models;
using PhoneCare_API.Data;
using PhoneCare_API.Models.DTO;
using PhoneCare_API.Services;

namespace PhoneCare_API.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly CurrentUserService _currentUserService;

        public ReportsController(ApplicationDbContext db, CurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        [HttpGet("don-hang/{id:int}/phieu-nhan-may")]
        public async Task<ActionResult<ApiResponse<DonHangReportDto>>> PhieuNhanMay(int id)
        {
            return await BuildReport(id, "Lấy dữ liệu phiếu nhận máy thành công.", includeNguoiThu: false);
        }

        [HttpGet("don-hang/{id:int}/hoa-don")]
        public async Task<ActionResult<ApiResponse<DonHangReportDto>>> HoaDon(int id)
        {
            return await BuildReport(id, "Lấy dữ liệu hóa đơn thành công.", includeNguoiThu: true);
        }

        private async Task<ActionResult<ApiResponse<DonHangReportDto>>> BuildReport(int id, string message, bool includeNguoiThu)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<DonHangReportDto>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanViewOrders(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<DonHangReportDto>.Forbidden("Bạn không có quyền xem báo cáo đơn hàng."));

            var order = await _db.DonHangs
                .Include(x => x.NhanVien)
                .Include(x => x.CoSoCuaHang)
                .Include(x => x.DichVus)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (order == null) return NotFound(ApiResponse<DonHangReportDto>.NotFound("Không tìm thấy đơn hàng."));

            var services = order.DichVus?
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Select((x, index) => new ReportDichVuLineDto
                {
                    STT = index + 1,
                    TenDichVu = x.TenDichVu,
                    DonGia = x.DonGia
                })
                .ToList() ?? new List<ReportDichVuLineDto>();

            var report = new DonHangReportDto
            {
                MaPhieu = order.Id.ToString(),
                ThoiGian = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                NguoiLap = current.FullName,
                NguoiThu = includeNguoiThu ? current.FullName : string.Empty,
                LoaiDichVu = order.LoaiDichVu,
                TenKH = order.TenKH,
                DiaChi = order.DiaChi,
                SoDT = order.SoDT,
                LoaiMay = order.LoaiMay,
                Mau = order.Mau,
                IMEI = order.IMEI,
                Password = order.Password,
                GhiChu = order.TinhTrangMay,
                TongTien = services.Sum(x => x.DonGia),
                NguoiNhanMay = order.NhanVien?.FullName ?? string.Empty,
                DiaChiCuaHang = order.CoSoCuaHang?.Address ?? string.Empty,
                DienThoaiCuaHang = GetStorePhone(order.CoSoCuaHang),
                DichVus = services
            };

            return Ok(ApiResponse<DonHangReportDto>.Ok(message, report));
        }

        private static string GetStorePhone(CoSoCuaHang? store)
        {
            if (store == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(store.Hotline)) return store.Hotline;
            return store.HomePhone ?? string.Empty;
        }
    }
}
