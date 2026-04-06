using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneCare.Models
{
    public class CoSoCuaHang
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string HomePhone { get; set; }
        public string Hotline { get; set; }
        public int IsDelete { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateModify { get; set; }
        public int UserCreated { get; set; }
        public int? UserModified { get; set; }

        // Navigation
        public virtual ICollection<NhanVien> NhanViens { get; set; }
    }
}
