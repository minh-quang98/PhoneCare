namespace PhoneCare.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class SecurityAndOrderFixes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DONHANG", "IMEI", c => c.String(maxLength: 20));

            AddColumn("dbo.NHANVIEN", "FailedLoginCount", c => c.Int(nullable: false));
            AddColumn("dbo.NHANVIEN", "LockoutEndAt", c => c.DateTime());
            AddColumn("dbo.NHANVIEN", "LastFailedLoginAt", c => c.DateTime());

            Sql("UPDATE dbo.DONHANG SET TinhTrang = 6 WHERE TinhTrang = 5");
            Sql("UPDATE dbo.DONHANG SET TinhTrang = 5 WHERE TinhTrang = 4");
            Sql("UPDATE dbo.DONHANG SET TinhTrang = 4 WHERE TinhTrang = 3");
        }

        public override void Down()
        {
            Sql("UPDATE dbo.DONHANG SET TinhTrang = 3 WHERE TinhTrang = 4");
            Sql("UPDATE dbo.DONHANG SET TinhTrang = 4 WHERE TinhTrang = 5");
            Sql("UPDATE dbo.DONHANG SET TinhTrang = 5 WHERE TinhTrang = 6");

            DropColumn("dbo.NHANVIEN", "LastFailedLoginAt");
            DropColumn("dbo.NHANVIEN", "LockoutEndAt");
            DropColumn("dbo.NHANVIEN", "FailedLoginCount");

            AlterColumn("dbo.DONHANG", "IMEI", c => c.Int(nullable: false));
        }
    }
}
