namespace PhoneCare.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_IsDelete_To_Bit : DbMigration
    {
        public override void Up()
        {
            // 1. Drop default constraint nếu có
            Sql(@"
        DECLARE @ConstraintName nvarchar(200)

        SELECT @ConstraintName = dc.name
        FROM sys.default_constraints dc
        JOIN sys.columns c 
            ON dc.parent_object_id = c.object_id 
            AND dc.parent_column_id = c.column_id
        WHERE OBJECT_NAME(dc.parent_object_id) = 'NHANVIEN'
        AND c.name = 'IsDeleted'

        IF @ConstraintName IS NOT NULL
            EXEC('ALTER TABLE NHANVIEN DROP CONSTRAINT ' + @ConstraintName)
    ");

            // 2. Chuẩn hóa dữ liệu
            Sql(@"
        UPDATE NHANVIEN
        SET IsDeleted = CASE 
            WHEN IsDeleted = 0 THEN 0 
            ELSE 1 
        END
    ");

            // 3. Đổi kiểu
            AlterColumn("dbo.NHANVIEN", "IsDeleted", c => c.Boolean(nullable: false));

            // 4. Add default
            Sql("ALTER TABLE NHANVIEN ADD DEFAULT 0 FOR IsDeleted");
        }
        
        public override void Down()
        {
        }
    }
}
