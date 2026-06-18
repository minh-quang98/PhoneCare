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

        public CoSoCuaHangController(ApplicationDbContext db, CurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CoSoCuaHangDTO>>>> GetAll()
        {
            var data = await _db.CoSoCuaHangs
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<CoSoCuaHangDTO>>> GetById(int id)
        {
            var item = await _db.CoSoCuaHangs.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (item == null) return NotFound(ApiResponse<CoSoCuaHangDTO>.NotFound("Khong tim thay co so cua hang."));
            return Ok(ApiResponse<CoSoCuaHangDTO>.Ok("Lay co so cua hang thanh cong.", Map(item)));
        }

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

        private static string? ValidateStore(string code, string name, string address)
        {
            if (string.IsNullOrWhiteSpace(code)) return "Code khong duoc de trong.";
            if (string.IsNullOrWhiteSpace(name)) return "Name khong duoc de trong.";
            if (string.IsNullOrWhiteSpace(address)) return "Address khong duoc de trong.";
            return null;
        }

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
