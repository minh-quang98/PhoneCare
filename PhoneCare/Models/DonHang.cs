using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneCare.Models
{
    public class DonHang
    {
        public int Id { get; set; }
        public string TenKH { get; set; }
        public string SoDT { get; set; }
        public string DiaChi { get; set; }
        public string LoaiMay { get; set; }
        public int IMEI { get; set; }
        public string May { get; set; }
        public string Password { get; set; }
        public int Level { get; set; }
        public string LoaiKyThuat { get; set; }
        public int TinhTrang { get; set; }
        public string TinhTrangMay { get; set; }
        public string LoaiDichVu { get; set; }
        public DateTime NgayNhan {  get; set; }
        public int IdNguoiNhan { get; set; }
        public int IsDelete { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateModify { get; set; }
        public int UserCreated { get; set; }
        public int? UserModified { get; set; }

        // Navigation
        public virtual NhanVien NhanVien { get; set; }
        public virtual ICollection<DichVu> DichVus { get; set; }
    }
}
