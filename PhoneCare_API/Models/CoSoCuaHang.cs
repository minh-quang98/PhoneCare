using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneCare.Models
{
    [Table("COSOCUAHANG")]
    public class CoSoCuaHang
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string Code { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Address { get; set; }
        public string HomePhone { get; set; }
        public string Hotline { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime? DateModify { get; set; }

        public int UserCreated { get; set; }
        public int? UserModify { get; set; }

        // Navigation
        public virtual ICollection<NhanVien> NhanViens { get; set; }
        public virtual ICollection<DonHang> DonHangs { get; set; }
    }
}
