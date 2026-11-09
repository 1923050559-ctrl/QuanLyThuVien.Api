using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Du lieu Admin gui len khi TAO tai khoan moi.
/// KHONG co MaTaiKhoan (do SERVER sinh) va KHONG co MatKhauBam (server tu bam tu MatKhau).
/// </summary>
public class TaiKhoanCreateDto
{
    // Danh sach TRANG: chi chu thuong khong dau, chu so, dau gach duoi, dau cham.
    // Khong co dau cach, khong co chu hoa -> "Admin" va "admin" khong the cung ton tai,
    // khong co ten gia mao bang ky tu nhin giong nhau.
    [Required(ErrorMessage = "Ten dang nhap khong duoc bo trong")]
    [RegularExpression("^[a-z0-9_.]{3,30}$",
        ErrorMessage = "Ten dang nhap 3-30 ky tu, chi gom chu thuong khong dau, chu so, dau _ va dau .")]
    [DefaultValue("thuthu01")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mat khau khong duoc bo trong")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mat khau tu 8 den 100 ky tu")]
    [DefaultValue("matkhau123")]
    public string MatKhau { get; set; } = string.Empty;

    // Mac dinh "User" (quyen thap nhat): bo quen truong nay thi KHONG vo tinh sinh ra Admin.
    [Required(ErrorMessage = "Vai tro khong duoc bo trong")]
    [RegularExpression("^(Admin|User)$", ErrorMessage = "Vai tro chi nhan Admin hoac User")]
    [DefaultValue("User")]
    public string VaiTro { get; set; } = "User";
}
