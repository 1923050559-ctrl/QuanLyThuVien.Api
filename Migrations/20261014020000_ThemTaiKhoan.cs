using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyThuVien.Api.Migrations
{
    /// <inheritdoc />
    public partial class ThemTaiKhoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenDangNhap = table.Column<string>(type: "TEXT", nullable: false),
                    MatKhauBam = table.Column<string>(type: "TEXT", nullable: false),
                    VaiTro = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                });

            migrationBuilder.InsertData(
                table: "TaiKhoan",
                columns: new[] { "MaTaiKhoan", "MatKhauBam", "TenDangNhap", "VaiTro" },
                values: new object[,]
                {
                    { 1, "JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=", "admin", "Admin" },
                    { 2, "5gbjiw2MGbJM8O44CBgxYup81j/3kS27IrXoAyhrREY=", "docgia", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaiKhoan");
        }
    }
}
