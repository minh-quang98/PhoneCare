using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneCare.Models
{
    public class NhanVien
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int IdCoSoLamViec { get; set; }
        public int KhoaTaiKhoan { get; set; } = 0;
        public string LoaiNhanVien { get; set; }
        public int IsDelete { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateModify { get; set; }
        public int UserCreated { get; set; }
        public int? UserModified { get; set; }

        // Navigation
        public virtual CoSoCuaHang CoSoCuaHang { get; set; }
        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}
