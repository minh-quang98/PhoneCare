using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneCare.Models
{
    [Table("DONHANG")]
    public class DonHang
    {
        [Key]
        public int Id { get; set; }

        public string TenKH { get; set; }
        public string SoDT { get; set; }
        public string DiaChi { get; set; }

        public string LoaiMay { get; set; }
        public string IMEI { get; set; }
        public string Mau { get; set; }
        public string Password { get; set; }

        public int Level { get; set; }
        public string LoaiKyThuat { get; set; }

        public string TinhTrang { get; set; }
        public string TinhTrangMay { get; set; }

        public string LoaiDichVu { get; set; }

        public DateTime? NgayNhan { get; set; }

        public int IdNguoiNhan { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateModify { get; set; }

        public int UserCreated { get; set; }
        public int? UserModify { get; set; }
        public int IdCoSo { get; set; }

        // Navigation
        [ForeignKey("IdNguoiNhan")]
        public virtual NhanVien NhanVien { get; set; }

        [ForeignKey("IdCoSo")]
        public virtual CoSoCuaHang CoSoCuaHang { get; set; }

        public virtual ICollection<DichVu> DichVus { get; set; }
    }
}
