using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneCare.Models
{
    [Table("DICHVU")]
    public class DichVu
    {
        [Key]
        public int Id { get; set; }

        public string TenDichVu { get; set; }
        public decimal DonGia { get; set; }

        public int IdDonHang { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateModify { get; set; }

        public int UserCreated { get; set; }
        public int? UserModify { get; set; }

        // Navigation
        [ForeignKey("IdDonHang")]
        public virtual DonHang DonHang { get; set; }
    }
}
