using System.Security.Cryptography;
using System.Text;

namespace QuanLyThuVien.Api.Helpers;

/// <summary>
/// LAB 9 (mo rong) - HAM BAM MAT KHAU DUNG CHUNG.
///
/// Truoc day ham nay nam trong AuthController (Phu luc 8.A.3). Tu khi co quan ly tai khoan,
/// ba noi can bam mat khau: dang nhap, doi mat khau (AuthController) va tao tai khoan,
/// dat lai mat khau (TaiKhoanController). Chep ham ra hai noi se de LECH nhau:
/// sua o mot noi quen noi kia thi tai khoan moi tao khong dang nhap duoc.
/// Gom ve MOT cho duy nhat.
///
/// MO HINH MINH HOA - he thong that dung bo bam chuyen cho mat khau co salt
/// (BCrypt, PBKDF2, Argon2) va kiem tra bang ham Verify cua thu vien (Giao trinh 8.4.2, Bai tap 8.6).
/// </summary>
public static class MatKhauHelper
{
    /// <summary>SHA256 roi chuyen ma Base64 - khop voi chuoi MatKhauBam nap san trong HasData.</summary>
    public static string Bam(string matKhau)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(matKhau));
        return Convert.ToBase64String(bytes);
    }
}
