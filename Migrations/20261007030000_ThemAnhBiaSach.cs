using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyThuVien.Api.Migrations
{
    /// <inheritdoc />
    public partial class ThemAnhBiaSach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DuongDanAnhBia",
                table: "Sach",
                type: "TEXT",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenAnhGoc",
                table: "Sach",
                type: "TEXT",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuongDanAnhBia",
                table: "Sach");

            migrationBuilder.DropColumn(
                name: "TenAnhGoc",
                table: "Sach");
        }
    }
}
