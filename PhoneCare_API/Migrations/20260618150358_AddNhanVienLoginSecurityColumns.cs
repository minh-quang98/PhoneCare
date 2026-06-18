using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhoneCare_API.Migrations
{
    /// <inheritdoc />
    public partial class AddNhanVienLoginSecurityColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
    name: "FailedLoginCount",
    table: "NHANVIEN",
    type: "int",
    nullable: false,
    defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEndAt",
                table: "NHANVIEN",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastFailedLoginAt",
                table: "NHANVIEN",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
    name: "FailedLoginCount",
    table: "NHANVIEN");

            migrationBuilder.DropColumn(
                name: "LockoutEndAt",
                table: "NHANVIEN");

            migrationBuilder.DropColumn(
                name: "LastFailedLoginAt",
                table: "NHANVIEN");
        }
    }
}
