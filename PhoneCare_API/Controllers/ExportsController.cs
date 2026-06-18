using System.Text;
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

        public ExportsController(ApplicationDbContext db, CurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

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

            var csv = new StringBuilder();
            csv.AppendLine("STT,ID,Ten KH,SDT,Loai may,IMEI,Ngay nhan,Nguoi nhan,Ky thuat,Trang thai,Level,Co so");
            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                csv.AppendLine(string.Join(",", new[]
                {
                    Csv(i + 1),
                    Csv(item.Id),
                    Csv(item.TenKH),
                    Csv(item.SoDT),
                    Csv(item.LoaiMay),
                    Csv(item.IMEI),
                    Csv(item.NgayNhan?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty),
                    Csv(item.NguoiNhan),
                    Csv(item.LoaiKyThuat),
                    Csv(RepairStatusService.GetText(item.TinhTrang)),
                    Csv(item.Level),
                    Csv(item.CoSo)
                }));
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
            return File(bytes, "text/csv; charset=utf-8", $"DanhSachDonHang_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

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

        private static string Csv(object? value)
        {
            var text = Convert.ToString(value) ?? string.Empty;
            return "\"" + text.Replace("\"", "\"\"") + "\"";
        }
    }
}
