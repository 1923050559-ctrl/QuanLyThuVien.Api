using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace QuanLyThuVien.Api.Models;

/// <summary>
/// LAB 8 - TAI KHOAN dang nhap. Giao trinh 8.4.3 va Phu luc 8.A.1.
///
/// Lab 8: hai tai khoan NAP SAN bang HasData trong AppDbContext (admin/admin123 va docgia/user123).
/// Lab 9 (mo rong): Admin tao, doi vai tro, dat lai mat khau, xoa tai khoan qua TaiKhoanController;
/// moi nguoi dung tu doi mat khau cua minh qua AuthController. Van KHONG co dang ky tu do:
/// tai khoan doc gia do thu vien cap.
///
/// KHONG BAO GIO luu mat khau dang van ban thuan - chi luu CHUOI BAM (Giao trinh 8.4.1).
/// </summary>
[Index(nameof(TenDangNhap), IsUnique = true)]   // Lab 9: ten dang nhap DUY NHAT - rang buoc o tang database
public class TaiKhoan
{
    // Ten khoa chinh khong theo quy uoc Id / TaiKhoanId -> bat buoc co [Key]
    [Key]
    public int MaTaiKhoan { get; set; }

    public string TenDangNhap { get; set; } = "";

    /// <summary>
    /// SHA256 roi chuyen ma Base64 - MO HINH MINH HOA (Giao trinh 8.4.2).
    /// He thong that dung bo bam chuyen cho mat khau co salt (Bai tap 8.6).
    /// </summary>
    public string MatKhauBam { get; set; } = "";

    /// <summary>Admin hoac User. Dua vao payload cua token - buoi 9 dung de phan quyen.</summary>
    public string VaiTro { get; set; } = "User";
}
