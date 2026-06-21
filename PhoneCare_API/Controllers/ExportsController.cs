using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneCare_API.Data;
using PhoneCare_API.Models.DTO;
using PhoneCare_API.Services;

namespace PhoneCare_API.Controllers
{
    [Route("api/exports")]
    [ApiController]
    public class ExportsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly CurrentUserService _currentUserService;

        /// <summary>
        /// Khởi tạo controller xuất dữ liệu cùng các dịch vụ phụ thuộc.
        /// </summary>
        public ExportsController(ApplicationDbContext db, CurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Lọc và xuất danh sách đơn hàng thành tệp Excel.
        /// </summary>
        [HttpGet("don-hang")]
        public async Task<IActionResult> ExportDonHang([FromQuery] DonHangQueryDto query)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<object>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanViewOrders(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.Forbidden("Bạn không có quyền xuất danh sách đơn hàng."));

            var data = await BuildFilteredQuery(query)
                .OrderByDescending(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.TenKH,
                    x.SoDT,
                    x.LoaiMay,
                    x.IMEI,
                    x.NgayNhan,
                    NguoiNhan = x.NhanVien != null ? x.NhanVien.FullName : string.Empty,
                    x.LoaiKyThuat,
                    x.TinhTrang,
                    x.Level,
                    CoSo = x.CoSoCuaHang != null ? x.CoSoCuaHang.Name : string.Empty
                })
                .ToListAsync();

            var headers = new[] { "STT", "ID", "Tên KH", "SĐT", "Loại máy", "IMEI", "Ngày nhận", "Người nhận", "Kỹ thuật", "Trạng thái", "Level", "Cơ sở" };
            var rows = new List<IReadOnlyList<object?>>(data.Count);
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                rows.Add(new object?[]
                {
                    i + 1,
                    item.Id,
                    item.TenKH,
                    item.SoDT,
                    item.LoaiMay,
                    item.IMEI,
                    item.NgayNhan,
                    item.NguoiNhan,
                    item.LoaiKyThuat,
                    RepairStatusService.GetText(item.TinhTrang),
                    item.Level,
                    item.CoSo
                });
            }

            var bytes = ExcelExportService.Create("Danh sách đơn hàng", headers, rows);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"DanhSachDonHang_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        /// <summary>
        /// Tạo truy vấn đơn hàng theo các điều kiện lọc được cung cấp.
        /// </summary>
        private IQueryable<PhoneCare.Models.DonHang> BuildFilteredQuery(DonHangQueryDto query)
        {
            var dbQuery = _db.DonHangs
                .Include(x => x.NhanVien)
                .Include(x => x.CoSoCuaHang)
                .Where(x => !x.IsDeleted);

            var keyword = query.Keyword?.Trim();
            var searchBy = query.SearchBy?.Trim();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                switch (searchBy)
                {
                    case "ID":
                    case "Id":
                        if (int.TryParse(keyword, out var id)) dbQuery = dbQuery.Where(x => x.Id == id);
                        break;
                    case "TenKH":
                    case "Tên KH":
                        dbQuery = dbQuery.Where(x => x.TenKH != null && x.TenKH.Contains(keyword));
                        break;
                    case "SoDT":
                    case "SĐT":
                    case "SDT":
                        dbQuery = dbQuery.Where(x => x.SoDT != null && x.SoDT.Contains(keyword));
                        break;
                    case "IMEI":
                        dbQuery = dbQuery.Where(x => x.IMEI != null && x.IMEI.Contains(keyword));
                        break;
                    case "KyThuat":
                    case "Kỹ thuật":
                        dbQuery = dbQuery.Where(x => x.LoaiKyThuat != null && x.LoaiKyThuat.Contains(keyword));
                        break;
                    case "LoaiMay":
                    case "Loại máy":
                        dbQuery = dbQuery.Where(x => x.LoaiMay != null && x.LoaiMay.Contains(keyword));
                        break;
                }
            }

            if (query.TinhTrang.HasValue) dbQuery = dbQuery.Where(x => x.TinhTrang == query.TinhTrang.Value);
            if (query.IdCoSo.HasValue) dbQuery = dbQuery.Where(x => x.IdCoSo == query.IdCoSo.Value);
            if (query.FromDate.HasValue) dbQuery = dbQuery.Where(x => x.NgayNhan >= query.FromDate.Value.Date);
            if (query.ToDate.HasValue) dbQuery = dbQuery.Where(x => x.NgayNhan < query.ToDate.Value.Date.AddDays(1));

            return dbQuery;
        }

    }
}
