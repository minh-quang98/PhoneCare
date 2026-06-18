namespace PhoneCare_API.Models.DTO
{
    public class CreateCoSoCuaHangDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? HomePhone { get; set; }
        public string? Hotline { get; set; }
    }

    public class UpdateCoSoCuaHangDto : CreateCoSoCuaHangDto
    {
    }

    public class NhanVienListItemDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string LoaiNhanVien { get; set; } = string.Empty;
        public int IdCoSoLamViec { get; set; }
        public string WorkPlaceName { get; set; } = string.Empty;
        public bool KhoaTaiKhoan { get; set; }
    }

    public class NhanVienDetailDto : NhanVienListItemDto
    {
        public string? NickName { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime? LockoutEndAt { get; set; }
        public DateTime? LastFailedLoginAt { get; set; }
    }

    public class CreateNhanVienDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? NickName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int IdCoSoLamViec { get; set; }
        public bool KhoaTaiKhoan { get; set; }
        public string LoaiNhanVien { get; set; } = string.Empty;
    }

    public class UpdateNhanVienDto
    {
        public string UserName { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? NickName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int IdCoSoLamViec { get; set; }
        public bool KhoaTaiKhoan { get; set; }
        public string LoaiNhanVien { get; set; } = string.Empty;
    }

    public class SetLockNhanVienDto
    {
        public bool KhoaTaiKhoan { get; set; }
    }

    public class ResetPasswordDto
    {
        public string Password { get; set; } = string.Empty;
    }

    public class DonHangQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchBy { get; set; }
        public string? Keyword { get; set; }
        public int? TinhTrang { get; set; }
        public int? IdCoSo { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class DonHangListItemDto
    {
        public int Id { get; set; }
        public string? TenKH { get; set; }
        public string? SoDT { get; set; }
        public string? LoaiMay { get; set; }
        public string? IMEI { get; set; }
        public DateTime? NgayNhan { get; set; }
        public string NguoiNhan { get; set; } = string.Empty;
        public string? LoaiKyThuat { get; set; }
        public int TinhTrang { get; set; }
        public string TinhTrangText { get; set; } = string.Empty;
        public int Level { get; set; }
        public int IdCoSo { get; set; }
        public string CoSoName { get; set; } = string.Empty;
    }

    public class DonHangDetailDto : DonHangListItemDto
    {
        public string? DiaChi { get; set; }
        public string? Mau { get; set; }
        public string? Password { get; set; }
        public string? TinhTrangMay { get; set; }
        public string? LoaiDichVu { get; set; }
        public int IdNguoiNhan { get; set; }
        public decimal TongTien { get; set; }
        public IReadOnlyList<DichVuDto> DichVus { get; set; } = Array.Empty<DichVuDto>();
    }

    public class CreateDonHangDto
    {
        public string TenKH { get; set; } = string.Empty;
        public string SoDT { get; set; } = string.Empty;
        public string? DiaChi { get; set; }
        public string LoaiMay { get; set; } = string.Empty;
        public string IMEI { get; set; } = string.Empty;
        public string? Mau { get; set; }
        public string? Password { get; set; }
        public int Level { get; set; }
        public string LoaiKyThuat { get; set; } = string.Empty;
        public int TinhTrang { get; set; }
        public string TinhTrangMay { get; set; } = string.Empty;
        public string? LoaiDichVu { get; set; }
    }

    public class UpdateDonHangDto : CreateDonHangDto
    {
    }

    public class UpdateDonHangStatusDto
    {
        public int TinhTrang { get; set; }
    }

    public class DichVuDto
    {
        public int Id { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public int IdDonHang { get; set; }
    }

    public class CreateDichVuDto
    {
        public string TenDichVu { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
    }

    public class UpdateDichVuDto : CreateDichVuDto
    {
    }

    public class ReportDichVuLineDto
    {
        public int STT { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
    }

    public class DonHangReportDto
    {
        public string MaPhieu { get; set; } = string.Empty;
        public string ThoiGian { get; set; } = string.Empty;
        public string NguoiLap { get; set; } = string.Empty;
        public string NguoiThu { get; set; } = string.Empty;
        public string? LoaiDichVu { get; set; }
        public string? TenKH { get; set; }
        public string? DiaChi { get; set; }
        public string? SoDT { get; set; }
        public string? LoaiMay { get; set; }
        public string? Mau { get; set; }
        public string? IMEI { get; set; }
        public string? Password { get; set; }
        public string? GhiChu { get; set; }
        public decimal TongTien { get; set; }
        public string NguoiNhanMay { get; set; } = string.Empty;
        public string DiaChiCuaHang { get; set; } = string.Empty;
        public string DienThoaiCuaHang { get; set; } = string.Empty;
        public IReadOnlyList<ReportDichVuLineDto> DichVus { get; set; } = Array.Empty<ReportDichVuLineDto>();
    }
}
