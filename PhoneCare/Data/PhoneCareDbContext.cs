using PhoneCare.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneCare.Data
{
    public class PhoneCareDbContext: DbContext
    {
        public PhoneCareDbContext(): base("name=PhoneCareDbContext")
        {

        }
        public DbSet<CoSoCuaHang> CoSoCuaHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<DichVu> DichVus { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // COSOCUAHANG - NHANVIEN
            modelBuilder.Entity<NhanVien>()
                .HasRequired(nv => nv.CoSoCuaHang)
                .WithMany(cs => cs.NhanViens)
                .HasForeignKey(nv => nv.IdCoSoLamViec)
                .WillCascadeOnDelete(false);

            // NHANVIEN - DONHANG
            modelBuilder.Entity<DonHang>()
                .HasRequired(dh => dh.NhanVien)
                .WithMany(nv => nv.DonHangs)
                .HasForeignKey(dh => dh.IdNguoiNhan)
                .WillCascadeOnDelete(false);

            // DONHANG - DICHVU
            modelBuilder.Entity<DichVu>()
                .HasRequired(dv => dv.DonHang)
                .WithMany(dh => dh.DichVus)
                .HasForeignKey(dv => dv.IdDonHang)
                .WillCascadeOnDelete(true);
        }
    }
}
