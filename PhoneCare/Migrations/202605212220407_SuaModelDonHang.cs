namespace PhoneCare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SuaModelDonHang : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DONHANG", "IMEI", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DONHANG", "IMEI", c => c.String());
        }
    }
}
