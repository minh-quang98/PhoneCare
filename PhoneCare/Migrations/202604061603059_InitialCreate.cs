namespace PhoneCare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CoSoCuaHangs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        Address = c.String(),
                        HomePhone = c.String(),
                        Hotline = c.String(),
                        IsDelete = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModify = c.DateTime(),
                        UserCreated = c.Int(nullable: false),
                        UserModified = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NhanViens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        FullName = c.String(),
                        NickName = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        IdCoSoLamViec = c.Int(nullable: false),
                        KhoaTaiKhoan = c.Int(nullable: false),
                        LoaiNhanVien = c.String(),
                        IsDelete = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModify = c.DateTime(),
                        UserCreated = c.Int(nullable: false),
                        UserModified = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CoSoCuaHangs", t => t.IdCoSoLamViec)
                .Index(t => t.IdCoSoLamViec);
            
            CreateTable(
                "dbo.DonHangs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenKH = c.String(),
                        SoDT = c.String(),
                        DiaChi = c.String(),
                        LoaiMay = c.String(),
                        IMEI = c.Int(nullable: false),
                        May = c.String(),
                        Password = c.String(),
                        Level = c.Int(nullable: false),
                        LoaiKyThuat = c.String(),
                        TinhTrang = c.Int(nullable: false),
                        TinhTrangMay = c.String(),
                        LoaiDichVu = c.String(),
                        NgayNhan = c.DateTime(nullable: false),
                        IdNguoiNhan = c.Int(nullable: false),
                        IsDelete = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModify = c.DateTime(),
                        UserCreated = c.Int(nullable: false),
                        UserModified = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NhanViens", t => t.IdNguoiNhan)
                .Index(t => t.IdNguoiNhan);
            
            CreateTable(
                "dbo.DichVus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TenDichVu = c.String(),
                        DongGia = c.Int(nullable: false),
                        IdDonHang = c.Int(nullable: false),
                        IsDelete = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModify = c.DateTime(),
                        UserCreated = c.Int(nullable: false),
                        UserModified = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DonHangs", t => t.IdDonHang, cascadeDelete: true)
                .Index(t => t.IdDonHang);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DonHangs", "IdNguoiNhan", "dbo.NhanViens");
            DropForeignKey("dbo.DichVus", "IdDonHang", "dbo.DonHangs");
            DropForeignKey("dbo.NhanViens", "IdCoSoLamViec", "dbo.CoSoCuaHangs");
            DropIndex("dbo.DichVus", new[] { "IdDonHang" });
            DropIndex("dbo.DonHangs", new[] { "IdNguoiNhan" });
            DropIndex("dbo.NhanViens", new[] { "IdCoSoLamViec" });
            DropTable("dbo.DichVus");
            DropTable("dbo.DonHangs");
            DropTable("dbo.NhanViens");
            DropTable("dbo.CoSoCuaHangs");
        }
    }
}
