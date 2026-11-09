using Microsoft.EntityFrameworkCore;
using QuanLyThuVien.Api.Models;

namespace QuanLyThuVien.Api.Data;

/// <summary>
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Moi DbSet tuong ung mot BANG trong co so du lieu
    public DbSet<TacGia> TacGia => Set<TacGia>();
    public DbSet<Sach> Sach => Set<Sach>();
    public DbSet<PhieuMuon> PhieuMuon => Set<PhieuMuon>();

    // LAB 8 - bang tai khoan dang nhap (Giao trinh 8.4.3, Phu luc 8.A.2)
    public DbSet<TaiKhoan> TaiKhoan => Set<TaiKhoan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // =================================================================
        //  NAP DU LIEU MAU BANG HasData
        // =================================================================

        // ---- 1. BANG CHA truoc ----
        modelBuilder.Entity<TacGia>().HasData(
            new TacGia { MaTacGia = 1, HoTen = "Nguyen Van A",
                         TieuSu = "Giang vien Cong nghe thong tin" },
            new TacGia { MaTacGia = 2, HoTen = "Tran Thi B",
                         TieuSu = "Chuyen gia co so du lieu" },
            new TacGia { MaTacGia = 3, HoTen = "Le Van C",
                         TieuSu = "Ky su phan mem" }
        );

        // ---- 2. BANG CON sau ----        
        modelBuilder.Entity<Sach>().HasData(
            new Sach { MaSach = 1, MaTacGia = 1,
                       TenSach = "Lap trinh C# co ban",
                       NamXuatBan = 2015, ConSan = false,
                       GiaNhap = 80000, DaKiemDuyet = false },
            new Sach { MaSach = 2, MaTacGia = 2,
                       TenSach = "Co so du lieu quan he",
                       NamXuatBan = 2016, ConSan = true,
                       GiaNhap = 85000, DaKiemDuyet = true },
            new Sach { MaSach = 3, MaTacGia = 3,
                       TenSach = "Lap trinh Web nang cao",
                       NamXuatBan = 2017, ConSan = true,
                       GiaNhap = 90000, DaKiemDuyet = true },
            new Sach { MaSach = 4, MaTacGia = 1,
                       TenSach = "Cau truc du lieu va giai thuat",
                       NamXuatBan = 2018, ConSan = false,
                       GiaNhap = 95000, DaKiemDuyet = true },
            new Sach { MaSach = 5, MaTacGia = 2,
                       TenSach = "Nhap mon tri tue nhan tao",
                       NamXuatBan = 2019, ConSan = true,
                       GiaNhap = 100000, DaKiemDuyet = false },
            new Sach { MaSach = 6, MaTacGia = 3,
                       TenSach = "Mang may tinh",
                       NamXuatBan = 2020, ConSan = true,
                       GiaNhap = 105000, DaKiemDuyet = true },
            new Sach { MaSach = 7, MaTacGia = 1,
                       TenSach = "He dieu hanh",
                       NamXuatBan = 2021, ConSan = false,
                       GiaNhap = 110000, DaKiemDuyet = true },
            new Sach { MaSach = 8, MaTacGia = 2,
                       TenSach = "Phan tich thiet ke he thong",
                       NamXuatBan = 2022, ConSan = true,
                       GiaNhap = 115000, DaKiemDuyet = true },
            new Sach { MaSach = 9, MaTacGia = 3,
                       TenSach = "Lap trinh huong doi tuong",
                       NamXuatBan = 2023, ConSan = true,
                       GiaNhap = 120000, DaKiemDuyet = false },
            new Sach { MaSach = 10, MaTacGia = 1,
                       TenSach = "Cong nghe phan mem",
                       NamXuatBan = 2024, ConSan = false,
                       GiaNhap = 125000, DaKiemDuyet = true },
            new Sach { MaSach = 11, MaTacGia = 2,
                       TenSach = "Tri tue nhan tao ung dung",
                       NamXuatBan = 2025, ConSan = true,
                       GiaNhap = 130000, DaKiemDuyet = true },
            new Sach { MaSach = 12, MaTacGia = 3,
                       TenSach = "Khai pha du lieu",
                       NamXuatBan = 2015, ConSan = true,
                       GiaNhap = 135000, DaKiemDuyet = true },
            new Sach { MaSach = 13, MaTacGia = 1,
                       TenSach = "Hoc may co ban",
                       NamXuatBan = 2016, ConSan = false,
                       GiaNhap = 140000, DaKiemDuyet = false },
            new Sach { MaSach = 14, MaTacGia = 2,
                       TenSach = "An toan thong tin",
                       NamXuatBan = 2017, ConSan = true,
                       GiaNhap = 145000, DaKiemDuyet = true },
            new Sach { MaSach = 15, MaTacGia = 3,
                       TenSach = "Dien toan dam may",
                       NamXuatBan = 2018, ConSan = true,
                       GiaNhap = 150000, DaKiemDuyet = true },
            new Sach { MaSach = 16, MaTacGia = 1,
                       TenSach = "Lap trinh di dong",
                       NamXuatBan = 2019, ConSan = false,
                       GiaNhap = 155000, DaKiemDuyet = true },
            new Sach { MaSach = 17, MaTacGia = 2,
                       TenSach = "Thiet ke giao dien nguoi dung",
                       NamXuatBan = 2020, ConSan = true,
                       GiaNhap = 160000, DaKiemDuyet = false },
            new Sach { MaSach = 18, MaTacGia = 3,
                       TenSach = "Kiem thu phan mem",
                       NamXuatBan = 2021, ConSan = true,
                       GiaNhap = 165000, DaKiemDuyet = true },
            new Sach { MaSach = 19, MaTacGia = 1,
                       TenSach = "Quan ly du an cong nghe",
                       NamXuatBan = 2022, ConSan = false,
                       GiaNhap = 170000, DaKiemDuyet = true },
            new Sach { MaSach = 20, MaTacGia = 2,
                       TenSach = "Do hoa may tinh",
                       NamXuatBan = 2023, ConSan = true,
                       GiaNhap = 175000, DaKiemDuyet = true },
            new Sach { MaSach = 21, MaTacGia = 3,
                       TenSach = "Xu ly anh so",
                       NamXuatBan = 2024, ConSan = true,
                       GiaNhap = 180000, DaKiemDuyet = false },
            new Sach { MaSach = 22, MaTacGia = 1,
                       TenSach = "He quan tri co so du lieu",
                       NamXuatBan = 2025, ConSan = false,
                       GiaNhap = 185000, DaKiemDuyet = true },
            new Sach { MaSach = 23, MaTacGia = 2,
                       TenSach = "Lap trinh Python",
                       NamXuatBan = 2015, ConSan = true,
                       GiaNhap = 190000, DaKiemDuyet = true },
            new Sach { MaSach = 24, MaTacGia = 3,
                       TenSach = "Phat trien ung dung Web",
                       NamXuatBan = 2016, ConSan = true,
                       GiaNhap = 195000, DaKiemDuyet = true },
            new Sach { MaSach = 25, MaTacGia = 1,
                       TenSach = "Toan roi rac",
                       NamXuatBan = 2017, ConSan = false,
                       GiaNhap = 200000, DaKiemDuyet = false }
        );

        // ---- 3. THUC THE GIAO DICH cuoi cung ----        
        // LAB 9: moi phieu co CHU SO HUU (MaTaiKhoan) de thu phan quyen theo chu so huu.
        //   Phieu 1 va 3 thuoc ve docgia (2); phieu 2 thuoc ve admin (1).
        //   -> docgia xem duoc 2 phieu, khong dong vao duoc phieu 2.
        modelBuilder.Entity<PhieuMuon>().HasData(
            new PhieuMuon { MaPhieu = 1, MaSach = 1, MaTaiKhoan = 2,
                            TenNguoiMuon = "Pham Van D",
                            NgayMuon = new DateTime(2026, 9, 1) },
            new PhieuMuon { MaPhieu = 2, MaSach = 3, MaTaiKhoan = 1,
                            TenNguoiMuon = "Hoang Thi E",
                            NgayMuon = new DateTime(2026, 9, 3),
                            NgayTra = new DateTime(2026, 9, 20) },
            new PhieuMuon { MaPhieu = 3, MaSach = 1, MaTaiKhoan = 2,
                            TenNguoiMuon = "Vo Van F",
                            NgayMuon = new DateTime(2026, 9, 5) }
        );

        // ---- 4. LAB 8 - HAI TAI KHOAN NAP SAN (Giao trinh 8.4.3, Phu luc 8.A.2) ----
        // MatKhauBam = Base64(SHA256(mat khau)) - khop voi ham MatKhauHelper.Bam (Helpers/MatKhauHelper.cs).
        //   admin  / admin123 -> vai tro Admin
        //   docgia / user123  -> vai tro User
        modelBuilder.Entity<TaiKhoan>().HasData(
            new TaiKhoan { MaTaiKhoan = 1, TenDangNhap = "admin", VaiTro = "Admin",
                MatKhauBam = "JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=" },
            new TaiKhoan { MaTaiKhoan = 2, TenDangNhap = "docgia", VaiTro = "User",
                MatKhauBam = "5gbjiw2MGbJM8O44CBgxYup81j/3kS27IrXoAyhrREY=" }
        );
    }
}
