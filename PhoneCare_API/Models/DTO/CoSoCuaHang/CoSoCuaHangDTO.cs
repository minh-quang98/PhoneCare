namespace PhoneCare_API.Models.DTO.CoSoCuaHang
{
    public class CoSoCuaHangDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string HomePhone { get; set; } = string.Empty;
        public string Hotline { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateModify { get; set; }

        public int UserCreated { get; set; }
        public int? UserModify { get; set; }
    }
}
