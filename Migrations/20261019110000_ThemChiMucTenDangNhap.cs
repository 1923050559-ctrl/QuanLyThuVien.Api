using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyThuVien.Api.Migrations
{
    /// <inheritdoc />
    public partial class ThemChiMucTenDangNhap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // LAB 9 - ten dang nhap DUY NHAT. Co quan ly tai khoan thi hai yeu cau tao trung ten
            // co the den cung luc; kiem tra bang ma trong action khong chan duoc, chi muc UNIQUE moi chan duoc.
            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan");
        }
    }
}
