using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyThuVien.Api.Migrations
{
    /// <inheritdoc />
    public partial class ThemMaTaiKhoanPhieuMuon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // LAB 9 - chu so huu cua phieu muon. Cot cho phep NULL: phieu cu chua co chu so huu.
            migrationBuilder.AddColumn<int>(
                name: "MaTaiKhoan",
                table: "PhieuMuon",
                type: "INTEGER",
                nullable: true);

            // HasData da doi: ba phieu mau duoc gan chu so huu
            migrationBuilder.UpdateData(
                table: "PhieuMuon",
                keyColumn: "MaPhieu",
                keyValue: 1,
                column: "MaTaiKhoan",
                value: 2);

            migrationBuilder.UpdateData(
                table: "PhieuMuon",
                keyColumn: "MaPhieu",
                keyValue: 2,
                column: "MaTaiKhoan",
                value: 1);

            migrationBuilder.UpdateData(
                table: "PhieuMuon",
                keyColumn: "MaPhieu",
                keyValue: 3,
                column: "MaTaiKhoan",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaTaiKhoan",
                table: "PhieuMuon");
        }
    }
}
