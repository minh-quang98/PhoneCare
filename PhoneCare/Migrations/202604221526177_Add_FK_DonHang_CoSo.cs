namespace PhoneCare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_FK_DonHang_CoSo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DONHANG", "IdCoSo", c => c.Int(nullable: false));
            CreateIndex("dbo.DONHANG", "IdCoSo");
            AddForeignKey("dbo.DONHANG", "IdCoSo", "dbo.COSOCUAHANG", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DONHANG", "IdCoSo", "dbo.COSOCUAHANG");
            DropIndex("dbo.DONHANG", new[] { "IdCoSo" });
            DropColumn("dbo.DONHANG", "IdCoSo");
        }
    }
}
