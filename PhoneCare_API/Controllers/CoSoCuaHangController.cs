using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneCare.Models;
using PhoneCare_API.Data;
using PhoneCare_API.Models.DTO;
using PhoneCare_API.Models.DTO.CoSoCuaHang;
using PhoneCare_API.Services;

namespace PhoneCare_API.Controllers
{
    [Route("api/co-so-cua-hang")]
    [ApiController]
    public class CoSoCuaHangController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly CurrentUserService _currentUserService;

        /// <summary>
        /// Khởi tạo controller quản lý cơ sở cửa hàng cùng các dịch vụ phụ thuộc.
        /// </summary>
        public CoSoCuaHangController(ApplicationDbContext db, CurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Lấy danh sách bản ghi hợp lệ theo từng trang và trả về cho client.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CoSoCuaHangDTO>>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var data = await _db.CoSoCuaHangs
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CoSoCuaHangDTO
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Address = x.Address,
                    HomePhone = x.HomePhone ?? string.Empty,
                    Hotline = x.Hotline ?? string.Empty,
                    IsDeleted = x.IsDeleted,
                    DateCreated = x.DateCreated,
                    DateModify = x.DateModify,
                    UserCreated = x.UserCreated,
                    UserModify = x.UserModify
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<CoSoCuaHangDTO>>.Ok("Lay danh sach co so cua hang thanh cong.", data));
        }

        /// <summary>
        /// Tìm và trả về chi tiết bản ghi theo mã định danh.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<CoSoCuaHangDTO>>> GetById(int id)
        {
            var item = await _db.CoSoCuaHangs.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<CoSoCuaHangDTO>.NotFound("Khong tim thay co so cua hang."));
            return Ok(ApiResponse<CoSoCuaHangDTO>.Ok("Lay co so cua hang thanh cong.", Map(item)));
        }

        /// <summary>
        /// Kiểm tra dữ liệu và tạo mới bản ghi tương ứng.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CoSoCuaHangDTO>>> Create(CreateCoSoCuaHangDto request)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<CoSoCuaHangDTO>.Unauthorized("Vui long dang nhap."));
            if (!PermissionService.CanManageStores(current.LoaiNhanVien))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<CoSoCuaHangDTO>.Forbidden("Ban khong co quyen quan ly cua hang."));
            }

            var validation = ValidateStore(request.Code, request.Name, request.Address);
            if (validation != null) return BadRequest(ApiResponse<CoSoCuaHangDTO>.BadRequest(validation));

            var code = request.Code.Trim();
            if (await _db.CoSoCuaHangs.AnyAsync(x => x.Code == code))
            {
                return Conflict(ApiResponse<CoSoCuaHangDTO>.Conflict("Code da ton tai."));
            }

            var item = new CoSoCuaHang
            {
                Code = code,
                Name = request.Name.Trim(),
                Address = request.Address.Trim(),
                HomePhone = request.HomePhone?.Trim(),
                Hotline = request.Hotline?.Trim(),
                IsDeleted = false,
                DateCreated = DateTime.Now,
                UserCreated = current.Id
            };

            _db.CoSoCuaHangs.Add(item);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(ApiResponse<CoSoCuaHangDTO>.Conflict("Code da ton tai."));
            }

            return StatusCode(StatusCodes.Status201Created, ApiResponse<CoSoCuaHangDTO>.Created("Them co so cua hang thanh cong.", Map(item)));
        }

        /// <summary>
        /// Kiểm tra dữ liệu và cập nhật bản ghi được yêu cầu.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<CoSoCuaHangDTO>>> Update(int id, UpdateCoSoCuaHangDto request)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<CoSoCuaHangDTO>.Unauthorized("Vui long dang nhap."));
            if (!PermissionService.CanManageStores(current.LoaiNhanVien))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<CoSoCuaHangDTO>.Forbidden("Ban khong co quyen quan ly cua hang."));
            }

            var item = await _db.CoSoCuaHangs.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<CoSoCuaHangDTO>.NotFound("Khong tim thay co so cua hang."));

            var validation = ValidateStore(request.Code, request.Name, request.Address);
            if (validation != null) return BadRequest(ApiResponse<CoSoCuaHangDTO>.BadRequest(validation));

            var code = request.Code.Trim();
            if (await _db.CoSoCuaHangs.AnyAsync(x => x.Id != id && x.Code == code))
            {
                return Conflict(ApiResponse<CoSoCuaHangDTO>.Conflict("Code da ton tai, ke ca trong du lieu da xoa mem."));
            }

            item.Code = code;
            item.Name = request.Name.Trim();
            item.Address = request.Address.Trim();
            item.HomePhone = request.HomePhone?.Trim();
            item.Hotline = request.Hotline?.Trim();
            item.DateModify = DateTime.Now;
            item.UserModify = current.Id;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(ApiResponse<CoSoCuaHangDTO>.Conflict("Code da ton tai."));
            }

            return Ok(ApiResponse<CoSoCuaHangDTO>.Ok("Cap nhat co so cua hang thanh cong.", Map(item)));
        }

        /// <summary>
        /// Kiểm tra quyền và thực hiện xóa mềm bản ghi được yêu cầu.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var current = _currentUserService.GetCurrentUser(HttpContext);
            if (current == null) return Unauthorized(ApiResponse<object>.Unauthorized("Vui long dang nhap."));
            if (!PermissionService.CanManageStores(current.LoaiNhanVien))
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.Forbidden("Ban khong co quyen quan ly cua hang."));
            }

            var item = await _db.CoSoCuaHangs.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<object>.NotFound("Khong tim thay co so cua hang."));

            item.IsDeleted = true;
            item.DateModify = DateTime.Now;
            item.UserModify = current.Id;
            await _db.SaveChangesAsync();
            return Ok(ApiResponse<object>.Ok("Xoa co so cua hang thanh cong."));
        }

        /// <summary>
        /// Kiểm tra các trường bắt buộc của cơ sở cửa hàng.
        /// </summary>
        private static string? ValidateStore(string code, string name, string address)
        {
            if (string.IsNullOrWhiteSpace(code)) return "Code khong duoc de trong.";
            if (string.IsNullOrWhiteSpace(name)) return "Name khong duoc de trong.";
            if (string.IsNullOrWhiteSpace(address)) return "Address khong duoc de trong.";
            return null;
        }

        /// <summary>
        /// Ánh xạ entity sang đối tượng dữ liệu trả về cho client.
        /// </summary>
        private static CoSoCuaHangDTO Map(CoSoCuaHang item)
        {
            return new CoSoCuaHangDTO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                Address = item.Address,
                HomePhone = item.HomePhone ?? string.Empty,
                Hotline = item.Hotline ?? string.Empty,
                IsDeleted = item.IsDeleted,
                DateCreated = item.DateCreated,
                DateModify = item.DateModify,
                UserCreated = item.UserCreated,
                UserModify = item.UserModify
            };
        }
    }
}
