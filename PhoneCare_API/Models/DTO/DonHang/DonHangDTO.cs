namespace PhoneCare_API.Models.DTO.DonHang
{
    public class DonHangDTO
    {
        public int Id { get; set; }

        public string TenKH { get; set; } = string.Empty;
        public string SoDT { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;

        public string LoaiMay { get; set; } = string.Empty;
        public string IMEI { get; set; } = string.Empty;
        public string Mau { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public int Level { get; set; }
        public string LoaiKyThuat { get; set; } = string.Empty;

        public int TinhTrang { get; set; }
        public string TinhTrangMay { get; set; } = string.Empty;

        public string LoaiDichVu { get; set; } = string.Empty;

        public DateTime? NgayNhan { get; set; }

        public int IdNguoiNhan { get; set; }
        public int IdCoSo { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateModify { get; set; }

        public int UserCreated { get; set; }
        public int? UserModify { get; set; }
    }
}
