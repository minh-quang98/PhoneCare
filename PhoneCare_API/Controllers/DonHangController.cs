using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneCare.Models;
using PhoneCare_API.Data;
using PhoneCare_API.Models.DTO;
using PhoneCare_API.Services;

namespace PhoneCare_API.Controllers
{
    [Route("api/don-hang")]
    [ApiController]
    public class DonHangController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly CurrentUserService _currentUserService;

        /// <summary>
        /// Khởi tạo controller quản lý đơn hàng cùng các dịch vụ phụ thuộc.
        /// </summary>
        public DonHangController(ApplicationDbContext db, CurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Lấy danh sách bản ghi hợp lệ và trả về cho client.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<DonHangListItemDto>>>> GetAll([FromQuery] DonHangQueryDto query)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<PagedResult<DonHangListItemDto>>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanViewOrders(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<PagedResult<DonHangListItemDto>>.Forbidden("Bạn không có quyền xem đơn hàng."));

            query.Page = Math.Max(query.Page, 1);
            query.PageSize = Math.Clamp(query.PageSize, 1, 100);

            var dbQuery = BuildFilteredQuery(query);
            var total = await dbQuery.CountAsync();
            var items = await dbQuery
                .OrderByDescending(x => x.Id)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new DonHangListItemDto
                {
                    Id = x.Id,
                    TenKH = x.TenKH,
                    SoDT = x.SoDT,
                    LoaiMay = x.LoaiMay,
                    IMEI = x.IMEI,
                    NgayNhan = x.NgayNhan,
                    NguoiNhan = x.NhanVien != null ? x.NhanVien.FullName : string.Empty,
                    LoaiKyThuat = x.LoaiKyThuat,
                    IdKyThuatVien = x.IdKyThuatVien,
                    KyThuatVien = x.KyThuatVien != null ? x.KyThuatVien.FullName : x.LoaiKyThuat,
                    TinhTrang = x.TinhTrang,
                    TinhTrangText = RepairStatusService.GetText(x.TinhTrang),
                    Level = x.Level,
                    IdCoSo = x.IdCoSo,
                    CoSoName = x.CoSoCuaHang != null ? x.CoSoCuaHang.Name : string.Empty
                })
                .ToListAsync();

            return Ok(ApiResponse<PagedResult<DonHangListItemDto>>.Ok("Lấy danh sách đơn hàng thành công.", new PagedResult<DonHangListItemDto>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = total
            }));
        }

        /// <summary>
        /// Tìm và trả về chi tiết bản ghi theo mã định danh.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<DonHangDetailDto>>> GetById(int id)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<DonHangDetailDto>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanViewOrders(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<DonHangDetailDto>.Forbidden("Bạn không có quyền xem đơn hàng."));

            var item = await _db.DonHangs
                .Include(x => x.NhanVien)
                .Include(x => x.KyThuatVien)
                .Include(x => x.CoSoCuaHang)
                .Include(x => x.DichVus)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (item == null) return NotFound(ApiResponse<DonHangDetailDto>.NotFound("Không tìm thấy đơn hàng."));
            return Ok(ApiResponse<DonHangDetailDto>.Ok("Lấy đơn hàng thành công.", MapDetail(item)));
        }

        /// <summary>
        /// Kiểm tra dữ liệu và tạo mới bản ghi tương ứng.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<DonHangDetailDto>>> Create(CreateDonHangDto request)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<DonHangDetailDto>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanEditOrders(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<DonHangDetailDto>.Forbidden("Bạn không có quyền thêm đơn hàng."));

            var validation = ValidateOrder(request);
            if (validation != null) return BadRequest(ApiResponse<DonHangDetailDto>.BadRequest(validation));

            var technician = await FindTechnicianAsync(request.IdKyThuatVien);
            if (technician == null)
            {
                return BadRequest(ApiResponse<DonHangDetailDto>.BadRequest("Kỹ thuật viên không tồn tại hoặc không còn hoạt động."));
            }

            var branchId = request.IdCoSo ?? current.CoSoCuaHangId;
            if (!await IsActiveBranchAsync(branchId))
            {
                return BadRequest(ApiResponse<DonHangDetailDto>.BadRequest("Cơ sở cửa hàng không tồn tại hoặc đã ngừng hoạt động."));
            }

            var item = new DonHang
            {
                TenKH = request.TenKH.Trim(),
                SoDT = request.SoDT.Trim(),
                DiaChi = request.DiaChi?.Trim(),
                LoaiMay = request.LoaiMay.Trim(),
                IMEI = request.IMEI.Trim(),
                Mau = request.Mau?.Trim(),
                Password = request.Password,
                Level = request.Level,
                LoaiKyThuat = technician.FullName,
                IdKyThuatVien = technician.Id,
                TinhTrang = request.TinhTrang,
                TinhTrangMay = request.TinhTrangMay.Trim(),
                LoaiDichVu = request.LoaiDichVu?.Trim(),
                NgayNhan = DateTime.Now,
                IdNguoiNhan = current.Id,
                IdCoSo = branchId,
                DateCreated = DateTime.Now,
                UserCreated = current.Id,
                IsDeleted = false
            };

            _db.DonHangs.Add(item);
            await _db.SaveChangesAsync();
            await LoadOrderReferences(item);

            return StatusCode(StatusCodes.Status201Created, ApiResponse<DonHangDetailDto>.Created("Thêm đơn hàng thành công.", MapDetail(item)));
        }

        /// <summary>
        /// Kiểm tra dữ liệu và cập nhật bản ghi được yêu cầu.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<DonHangDetailDto>>> Update(int id, UpdateDonHangDto request)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<DonHangDetailDto>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanEditOrders(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<DonHangDetailDto>.Forbidden("Bạn không có quyền sửa đơn hàng."));

            var item = await _db.DonHangs.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<DonHangDetailDto>.NotFound("Không tìm thấy đơn hàng."));
            if (!RepairStatusService.CanEditOrder(item.TinhTrang)) return BadRequest(ApiResponse<DonHangDetailDto>.BadRequest("Không thể sửa đơn hàng ở trạng thái hiện tại."));

            var validation = ValidateOrder(request);
            if (validation != null) return BadRequest(ApiResponse<DonHangDetailDto>.BadRequest(validation));

            var technician = await FindTechnicianAsync(request.IdKyThuatVien);
            if (technician == null)
            {
                return BadRequest(ApiResponse<DonHangDetailDto>.BadRequest("Kỹ thuật viên không tồn tại hoặc không còn hoạt động."));
            }

            var branchId = request.IdCoSo ?? item.IdCoSo;
            if (!await IsActiveBranchAsync(branchId))
            {
                return BadRequest(ApiResponse<DonHangDetailDto>.BadRequest("Cơ sở cửa hàng không tồn tại hoặc đã ngừng hoạt động."));
            }

            item.TenKH = request.TenKH.Trim();
            item.SoDT = request.SoDT.Trim();
            item.DiaChi = request.DiaChi?.Trim();
            item.LoaiMay = request.LoaiMay.Trim();
            item.IMEI = request.IMEI.Trim();
            item.Mau = request.Mau?.Trim();
            item.Password = request.Password;
            item.Level = request.Level;
            item.LoaiKyThuat = technician.FullName;
            item.IdKyThuatVien = technician.Id;
            item.IdCoSo = branchId;
            item.TinhTrang = request.TinhTrang;
            item.TinhTrangMay = request.TinhTrangMay.Trim();
            item.LoaiDichVu = request.LoaiDichVu?.Trim();
            item.DateModify = DateTime.Now;
            item.UserModify = current.Id;
            await _db.SaveChangesAsync();
            await LoadOrderReferences(item);

            return Ok(ApiResponse<DonHangDetailDto>.Ok("Cập nhật đơn hàng thành công.", MapDetail(item)));
        }

        /// <summary>
        /// Kiểm tra và cập nhật trạng thái sửa chữa của đơn hàng.
        /// </summary>
        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<ApiResponse<DonHangDetailDto>>> UpdateStatus(int id, UpdateDonHangStatusDto request)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<DonHangDetailDto>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanEditOrders(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<DonHangDetailDto>.Forbidden("Bạn không có quyền sửa đơn hàng."));
            if (!RepairStatusService.IsValid(request.TinhTrang)) return BadRequest(ApiResponse<DonHangDetailDto>.BadRequest("Trạng thái không hợp lệ."));

            var item = await _db.DonHangs.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<DonHangDetailDto>.NotFound("Không tìm thấy đơn hàng."));

            item.TinhTrang = request.TinhTrang;
            item.DateModify = DateTime.Now;
            item.UserModify = current.Id;
            await _db.SaveChangesAsync();
            await LoadOrderReferences(item);

            return Ok(ApiResponse<DonHangDetailDto>.Ok("Cập nhật trạng thái đơn hàng thành công.", MapDetail(item)));
        }

        /// <summary>
        /// Kiểm tra quyền và thực hiện xóa mềm bản ghi được yêu cầu.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<object>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanEditOrders(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.Forbidden("Bạn không có quyền xóa đơn hàng."));

            var item = await _db.DonHangs.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<object>.NotFound("Không tìm thấy đơn hàng."));

            item.IsDeleted = true;
            item.DateModify = DateTime.Now;
            item.UserModify = current.Id;
            await _db.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok("Xóa đơn hàng thành công."));
        }

        /// <summary>
        /// Lấy danh sách dịch vụ thuộc đơn hàng được yêu cầu.
        /// </summary>
        [HttpGet("{donHangId:int}/dich-vu")]
        public async Task<ActionResult<ApiResponse<IEnumerable<DichVuDto>>>> GetServices(
            int donHangId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<IEnumerable<DichVuDto>>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanViewOrders(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<IEnumerable<DichVuDto>>.Forbidden("Bạn không có quyền xem đơn hàng."));

            if (!await _db.DonHangs.AnyAsync(x => x.Id == donHangId && !x.IsDeleted)) return NotFound(ApiResponse<IEnumerable<DichVuDto>>.NotFound("Không tìm thấy đơn hàng."));
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var data = await _db.DichVus
                .Where(x => x.IdDonHang == donHangId && !x.IsDeleted)
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new DichVuDto { Id = x.Id, TenDichVu = x.TenDichVu, DonGia = x.DonGia, IdDonHang = x.IdDonHang })
                .ToListAsync();
            return Ok(ApiResponse<IEnumerable<DichVuDto>>.Ok("Lấy danh sách dịch vụ thành công.", data));
        }

        /// <summary>
        /// Kiểm tra dữ liệu và thêm dịch vụ mới vào đơn hàng.
        /// </summary>
        [HttpPost("{donHangId:int}/dich-vu")]
        public async Task<ActionResult<ApiResponse<DichVuDto>>> CreateService(int donHangId, CreateDichVuDto request)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<DichVuDto>.Unauthorized("Vui lòng đăng nhập."));
            if (!PermissionService.CanManageServices(current.LoaiNhanVien)) return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<DichVuDto>.Forbidden("Bạn không có quyền cập nhật dịch vụ."));

            var order = await _db.DonHangs.FirstOrDefaultAsync(x => x.Id == donHangId && !x.IsDeleted);
            if (order == null) return NotFound(ApiResponse<DichVuDto>.NotFound("Không tìm thấy đơn hàng."));
            if (!RepairStatusService.CanEditOrder(order.TinhTrang)) return BadRequest(ApiResponse<DichVuDto>.BadRequest("Không thể cập nhật dịch vụ ở trạng thái hiện tại."));
            var validation = ValidateService(request.TenDichVu, request.DonGia);
            if (validation != null) return BadRequest(ApiResponse<DichVuDto>.BadRequest(validation));

            var service = new DichVu
            {
                TenDichVu = request.TenDichVu.Trim(),
                DonGia = request.DonGia,
                IdDonHang = donHangId,
                DateCreated = DateTime.Now,
                UserCreated = current.Id,
                IsDeleted = false
            };
            _db.DichVus.Add(service);
            await _db.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, ApiResponse<DichVuDto>.Created("Thêm dịch vụ thành công.", MapService(service)));
        }

        /// <summary>
        /// Tạo truy vấn đơn hàng theo các điều kiện lọc được cung cấp.
        /// </summary>
        private IQueryable<DonHang> BuildFilteredQuery(DonHangQueryDto query)
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

        /// <summary>
        /// Kiểm tra tính hợp lệ của dữ liệu đơn hàng.
        /// </summary>
        private static string? ValidateOrder(CreateDonHangDto request)
        {
            if (string.IsNullOrWhiteSpace(request.TenKH)) return "Tên khách hàng không được để trống.";
            if (string.IsNullOrWhiteSpace(request.SoDT)) return "Số điện thoại không được để trống.";
            if (!request.SoDT.All(char.IsDigit)) return "Số điện thoại chỉ được nhập số.";
            if (string.IsNullOrWhiteSpace(request.LoaiMay)) return "Loại máy không được để trống.";
            if (string.IsNullOrWhiteSpace(request.IMEI)) return "IMEI không được để trống.";
            if (!request.IMEI.All(char.IsDigit) || request.IMEI.Length < 14 || request.IMEI.Length > 17) return "IMEI phải là dãy số từ 14 đến 17 chữ số.";
            if (string.IsNullOrWhiteSpace(request.TinhTrangMay)) return "Tình trạng máy không được để trống.";
            if (!request.IdKyThuatVien.HasValue || request.IdKyThuatVien.Value <= 0) return "Vui lòng chọn kỹ thuật viên.";
            if (request.Level < 1 || request.Level > 10) return "Level phải từ 1 đến 10.";
            if (!RepairStatusService.IsValid(request.TinhTrang)) return "Trạng thái không hợp lệ.";
            return null;
        }

        /// <summary>
        /// Kiểm tra tên và đơn giá của dịch vụ.
        /// </summary>
        internal static string? ValidateService(string tenDichVu, decimal donGia)
        {
            if (string.IsNullOrWhiteSpace(tenDichVu)) return "Tên dịch vụ không được để trống.";
            if (donGia < 0) return "Báo giá không được nhỏ hơn 0.";
            return null;
        }

        /// <summary>
        /// Tải các quan hệ cần thiết của đơn hàng để ánh xạ dữ liệu trả về.
        /// </summary>
        private async Task LoadOrderReferences(DonHang item)
        {
            await _db.Entry(item).Reference(x => x.NhanVien).LoadAsync();
            await _db.Entry(item).Reference(x => x.KyThuatVien).LoadAsync();
            await _db.Entry(item).Reference(x => x.CoSoCuaHang).LoadAsync();
            await _db.Entry(item).Collection(x => x.DichVus).LoadAsync();
        }

        private Task<NhanVien?> FindTechnicianAsync(int? technicianId)
        {
            if (!technicianId.HasValue || technicianId.Value <= 0)
            {
                return Task.FromResult<NhanVien?>(null);
            }

            return _db.NhanViens.FirstOrDefaultAsync(x =>
                x.Id == technicianId.Value &&
                !x.IsDeleted &&
                !x.KhoaTaiKhoan &&
                x.LoaiNhanVien == PermissionService.KyThuat);
        }

        private Task<bool> IsActiveBranchAsync(int branchId)
        {
            return _db.CoSoCuaHangs.AnyAsync(x => x.Id == branchId && !x.IsDeleted);
        }

        /// <summary>
        /// Ánh xạ entity dịch vụ sang DTO dịch vụ.
        /// </summary>
        internal static DichVuDto MapService(DichVu item)
        {
            return new DichVuDto
            {
                Id = item.Id,
                TenDichVu = item.TenDichVu,
                DonGia = item.DonGia,
                IdDonHang = item.IdDonHang
            };
        }

        /// <summary>
        /// Ánh xạ entity sang DTO chi tiết.
        /// </summary>
        private static DonHangDetailDto MapDetail(DonHang item)
        {
            var services = item.DichVus?.Where(x => !x.IsDeleted).OrderBy(x => x.Id).Select(MapService).ToList() ?? new List<DichVuDto>();
            return new DonHangDetailDto
            {
                Id = item.Id,
                TenKH = item.TenKH,
                SoDT = item.SoDT,
                DiaChi = item.DiaChi,
                LoaiMay = item.LoaiMay,
                IMEI = item.IMEI,
                Mau = item.Mau,
                Password = item.Password,
                NgayNhan = item.NgayNhan,
                NguoiNhan = item.NhanVien?.FullName ?? string.Empty,
                LoaiKyThuat = item.LoaiKyThuat,
                IdKyThuatVien = item.IdKyThuatVien,
                KyThuatVien = item.KyThuatVien?.FullName ?? item.LoaiKyThuat,
                TinhTrang = item.TinhTrang,
                TinhTrangText = RepairStatusService.GetText(item.TinhTrang),
                TinhTrangMay = item.TinhTrangMay,
                LoaiDichVu = item.LoaiDichVu,
                Level = item.Level,
                IdNguoiNhan = item.IdNguoiNhan,
                IdCoSo = item.IdCoSo,
                CoSoName = item.CoSoCuaHang?.Name ?? string.Empty,
                TongTien = services.Sum(x => x.DonGia),
                DichVus = services
            };
        }
    }
}
