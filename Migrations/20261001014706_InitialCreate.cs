using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyThuVien.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TacGia",
                columns: table => new
                {
                    MaTacGia = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HoTen = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TieuSu = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TacGia", x => x.MaTacGia);
                });

            migrationBuilder.CreateTable(
                name: "Sach",
                columns: table => new
                {
                    MaSach = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenSach = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    NamXuatBan = table.Column<int>(type: "INTEGER", nullable: false),
                    ConSan = table.Column<bool>(type: "INTEGER", nullable: false),
                    GiaNhap = table.Column<decimal>(type: "TEXT", nullable: false),
                    DaKiemDuyet = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaTacGia = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sach", x => x.MaSach);
                    table.ForeignKey(
                        name: "FK_Sach_TacGia_MaTacGia",
                        column: x => x.MaTacGia,
                        principalTable: "TacGia",
                        principalColumn: "MaTacGia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuMuon",
                columns: table => new
                {
                    MaPhieu = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenNguoiMuon = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NgayMuon = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NgayTra = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HanTra = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MaSach = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuMuon", x => x.MaPhieu);
                    table.ForeignKey(
                        name: "FK_PhieuMuon_Sach_MaSach",
                        column: x => x.MaSach,
                        principalTable: "Sach",
                        principalColumn: "MaSach",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TacGia",
                columns: new[] { "MaTacGia", "HoTen", "TieuSu" },
                values: new object[,]
                {
                    { 1, "Nguyen Van A", "Giang vien Cong nghe thong tin" },
                    { 2, "Tran Thi B", "Chuyen gia co so du lieu" },
                    { 3, "Le Van C", "Ky su phan mem" }
                });

            migrationBuilder.InsertData(
                table: "Sach",
                columns: new[] { "MaSach", "ConSan", "DaKiemDuyet", "GiaNhap", "MaTacGia", "NamXuatBan", "TenSach" },
                values: new object[,]
                {
                    { 1, false, false, 80000m, 1, 2015, "Lap trinh C# co ban" },
                    { 2, true, true, 85000m, 2, 2016, "Co so du lieu quan he" },
                    { 3, true, true, 90000m, 3, 2017, "Lap trinh Web nang cao" },
                    { 4, false, true, 95000m, 1, 2018, "Cau truc du lieu va giai thuat" },
                    { 5, true, false, 100000m, 2, 2019, "Nhap mon tri tue nhan tao" },
                    { 6, true, true, 105000m, 3, 2020, "Mang may tinh" },
                    { 7, false, true, 110000m, 1, 2021, "He dieu hanh" },
                    { 8, true, true, 115000m, 2, 2022, "Phan tich thiet ke he thong" },
                    { 9, true, false, 120000m, 3, 2023, "Lap trinh huong doi tuong" },
                    { 10, false, true, 125000m, 1, 2024, "Cong nghe phan mem" },
                    { 11, true, true, 130000m, 2, 2025, "Tri tue nhan tao ung dung" },
                    { 12, true, true, 135000m, 3, 2015, "Khai pha du lieu" },
                    { 13, false, false, 140000m, 1, 2016, "Hoc may co ban" },
                    { 14, true, true, 145000m, 2, 2017, "An toan thong tin" },
                    { 15, true, true, 150000m, 3, 2018, "Dien toan dam may" },
                    { 16, false, true, 155000m, 1, 2019, "Lap trinh di dong" },
                    { 17, true, false, 160000m, 2, 2020, "Thiet ke giao dien nguoi dung" },
                    { 18, true, true, 165000m, 3, 2021, "Kiem thu phan mem" },
                    { 19, false, true, 170000m, 1, 2022, "Quan ly du an cong nghe" },
                    { 20, true, true, 175000m, 2, 2023, "Do hoa may tinh" },
                    { 21, true, false, 180000m, 3, 2024, "Xu ly anh so" },
                    { 22, false, true, 185000m, 1, 2025, "He quan tri co so du lieu" },
                    { 23, true, true, 190000m, 2, 2015, "Lap trinh Python" },
                    { 24, true, true, 195000m, 3, 2016, "Phat trien ung dung Web" },
                    { 25, false, false, 200000m, 1, 2017, "Toan roi rac" }
                });

            migrationBuilder.InsertData(
                table: "PhieuMuon",
                columns: new[] { "MaPhieu", "HanTra", "MaSach", "NgayMuon", "NgayTra", "TenNguoiMuon" },
                values: new object[,]
                {
                    { 1, null, 1, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Pham Van D" },
                    { 2, null, 3, new DateTime(2026, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hoang Thi E" },
                    { 3, null, 1, new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Vo Van F" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhieuMuon_MaSach",
                table: "PhieuMuon",
                column: "MaSach");

            migrationBuilder.CreateIndex(
                name: "IX_Sach_MaTacGia",
                table: "Sach",
                column: "MaTacGia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhieuMuon");

            migrationBuilder.DropTable(
                name: "Sach");

            migrationBuilder.DropTable(
                name: "TacGia");
        }
    }
}
