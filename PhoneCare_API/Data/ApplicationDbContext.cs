using Microsoft.EntityFrameworkCore;
using PhoneCare.Models;

namespace PhoneCare_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<CoSoCuaHang> CoSoCuaHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<DichVu> DichVus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CoSoCuaHang>().ToTable("COSOCUAHANG");
            modelBuilder.Entity<NhanVien>().ToTable("NHANVIEN");
            modelBuilder.Entity<DonHang>().ToTable("DONHANG");
            modelBuilder.Entity<DichVu>().ToTable("DICHVU");

            modelBuilder.Entity<DichVu>()
                .Property(dv => dv.DonGia)
                .HasPrecision(18, 2);

            // COSOCUAHANG - NHANVIEN
            modelBuilder.Entity<NhanVien>()
                .HasOne(nv => nv.CoSoCuaHang)
                .WithMany(cs => cs.NhanViens)
                .HasForeignKey(nv => nv.IdCoSoLamViec)
                .OnDelete(DeleteBehavior.Restrict);

            // NHANVIEN - DONHANG
            modelBuilder.Entity<DonHang>()
                .HasOne(dh => dh.NhanVien)
                .WithMany(nv => nv.DonHangsNhan)
                .HasForeignKey(dh => dh.IdNguoiNhan)
                .OnDelete(DeleteBehavior.Restrict);

            // DONHANG - DICHVU
            modelBuilder.Entity<DichVu>()
                .HasOne(dv => dv.DonHang)
                .WithMany(dh => dh.DichVus)
                .HasForeignKey(dv => dv.IdDonHang)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DonHang>()
                .HasOne(d => d.CoSoCuaHang)
                .WithMany(c => c.DonHangs)
                .HasForeignKey(d => d.IdCoSo)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
