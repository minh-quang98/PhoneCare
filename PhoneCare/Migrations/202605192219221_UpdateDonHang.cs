namespace PhoneCare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDonHang : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DONHANG", "TinhTrang", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DONHANG", "TinhTrang", c => c.String());
        }
    }
}
