using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhoneCare_API.Migrations
{
    /// <inheritdoc />
    public partial class AddDonHangTechnicianRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdKyThuatVien",
                table: "DONHANG",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DONHANG_IdKyThuatVien",
                table: "DONHANG",
                column: "IdKyThuatVien");

            migrationBuilder.AddForeignKey(
                name: "FK_DONHANG_NHANVIEN_IdKyThuatVien",
                table: "DONHANG",
                column: "IdKyThuatVien",
                principalTable: "NHANVIEN",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DONHANG_NHANVIEN_IdKyThuatVien",
                table: "DONHANG");

            migrationBuilder.DropIndex(
                name: "IX_DONHANG_IdKyThuatVien",
                table: "DONHANG");

            migrationBuilder.DropColumn(
                name: "IdKyThuatVien",
                table: "DONHANG");
        }
    }
}
