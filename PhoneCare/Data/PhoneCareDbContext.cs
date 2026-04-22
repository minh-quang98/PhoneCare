using PhoneCare.Models;
using System.Data.Entity;

namespace PhoneCare.Data
{
    public class PhoneCareDbContext : DbContext
    {
        public PhoneCareDbContext() : base("name=PhoneCareDbContext")
        {

        }
        public DbSet<CoSoCuaHang> CoSoCuaHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<DichVu> DichVus { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CoSoCuaHang>().ToTable("COSOCUAHANG");
            modelBuilder.Entity<NhanVien>().ToTable("NHANVIEN");
            modelBuilder.Entity<DonHang>().ToTable("DONHANG");
            modelBuilder.Entity<DichVu>().ToTable("DICHVU");

            // COSOCUAHANG - NHANVIEN
            modelBuilder.Entity<NhanVien>()
                .HasRequired(nv => nv.CoSoCuaHang)
                .WithMany(cs => cs.NhanViens)
                .HasForeignKey(nv => nv.IdCoSoLamViec)
                .WillCascadeOnDelete(false);

            // NHANVIEN - DONHANG
            modelBuilder.Entity<DonHang>()
                .HasRequired(dh => dh.NhanVien)
                .WithMany(nv => nv.DonHangsNhan)
                .HasForeignKey(dh => dh.IdNguoiNhan)
                .WillCascadeOnDelete(false);

            // DONHANG - DICHVU
            modelBuilder.Entity<DichVu>()
                .HasRequired(dv => dv.DonHang)
                .WithMany(dh => dh.DichVus)
                .HasForeignKey(dv => dv.IdDonHang)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<DonHang>()
                .HasRequired(d => d.CoSoCuaHang)
                .WithMany(c => c.DonHangs)
                .HasForeignKey(d => d.IdCoSo)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
