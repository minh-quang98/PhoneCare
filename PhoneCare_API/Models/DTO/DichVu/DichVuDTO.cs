namespace PhoneCare_API.Models.DTO.DichVu
{
    public class DichVuDTO
    {
        public int Id { get; set; }
        public string TenDichVu { get; set; } = string.Empty;
        public decimal DonGia { get; set; }
        public int IdDonHang { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateModify { get; set; }

        public int UserCreated { get; set; }
        public int? UserModify { get; set; }
    }
}
