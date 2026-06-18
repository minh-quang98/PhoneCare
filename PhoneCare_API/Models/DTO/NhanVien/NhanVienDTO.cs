namespace PhoneCare_API.Models.DTO.NhanVien
{
    public class NhanVienDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int IdCoSoLamViec { get; set; }
        public bool KhoaTaiKhoan { get; set; }
        public string LoaiNhanVien { get; set; } = string.Empty;
        public int FailedLoginCount { get; set; }
        public DateTime? LockoutEndAt { get; set; }
        public DateTime? LastFailedLoginAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModify { get; set; }
        public int UserCreated { get; set; }
        public int? UserModify { get; set; }
    }
}
