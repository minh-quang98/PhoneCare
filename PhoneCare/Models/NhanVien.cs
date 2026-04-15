using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneCare.Models
{
    [Table("NHANVIEN")]
    public class NhanVien
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int IdCoSoLamViec { get; set; }
        public bool KhoaTaiKhoan { get; set; }
        public string LoaiNhanVien { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModify { get; set; }
        public int UserCreated { get; set; }
        public int? UserModify { get; set; }

        // Navigation
        [ForeignKey("IdCoSoLamViec")]
        public virtual CoSoCuaHang CoSoCuaHang { get; set; }

        public virtual ICollection<DonHang> DonHangsNhan { get; set; }
    }
}
