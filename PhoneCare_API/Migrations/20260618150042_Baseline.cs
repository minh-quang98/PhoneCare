using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhoneCare_API.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "COSOCUAHANG",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomePhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Hotline = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserModify = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COSOCUAHANG", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NHANVIEN",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdCoSoLamViec = table.Column<int>(type: "int", nullable: false),
                    KhoaTaiKhoan = table.Column<bool>(type: "bit", nullable: false),
                    LoaiNhanVien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserModify = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHANVIEN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NHANVIEN_COSOCUAHANG_IdCoSoLamViec",
                        column: x => x.IdCoSoLamViec,
                        principalTable: "COSOCUAHANG",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DONHANG",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiMay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IMEI = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    LoaiKyThuat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TinhTrang = table.Column<int>(type: "int", nullable: false),
                    TinhTrangMay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayNhan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdNguoiNhan = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserModify = table.Column<int>(type: "int", nullable: true),
                    IdCoSo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DONHANG", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DONHANG_COSOCUAHANG_IdCoSo",
                        column: x => x.IdCoSo,
                        principalTable: "COSOCUAHANG",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DONHANG_NHANVIEN_IdNguoiNhan",
                        column: x => x.IdNguoiNhan,
                        principalTable: "NHANVIEN",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DICHVU",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdDonHang = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModify = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserCreated = table.Column<int>(type: "int", nullable: false),
                    UserModify = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DICHVU", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DICHVU_DONHANG_IdDonHang",
                        column: x => x.IdDonHang,
                        principalTable: "DONHANG",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DICHVU_IdDonHang",
                table: "DICHVU",
                column: "IdDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_DONHANG_IdCoSo",
                table: "DONHANG",
                column: "IdCoSo");

            migrationBuilder.CreateIndex(
                name: "IX_DONHANG_IdNguoiNhan",
                table: "DONHANG",
                column: "IdNguoiNhan");

            migrationBuilder.CreateIndex(
                name: "IX_NHANVIEN_IdCoSoLamViec",
                table: "NHANVIEN",
                column: "IdCoSoLamViec");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DICHVU");
            migrationBuilder.DropTable(name: "DONHANG");
            migrationBuilder.DropTable(name: "NHANVIEN");
            migrationBuilder.DropTable(name: "COSOCUAHANG");

        }
    }
}
