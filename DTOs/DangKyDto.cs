using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Du lieu khach hang / doc gia gui len khi DANG KY tai khoan moi.
/// TUYET DOI khong co truong VaiTro: server luon mac dinh gan vai tro "User",
/// tranh tan cong over-posting de tu cap quyen Admin.
/// </summary>
public class DangKyDto
{
    [Required(ErrorMessage = "Ten dang nhap khong duoc bo trong")]
    [RegularExpression("^[a-z0-9_.]{3,30}$",
        ErrorMessage = "Ten dang nhap 3-30 ky tu, chi gom chu thuong khong dau, chu so, dau _ va dau .")]
    [DefaultValue("docgia01")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mat khau khong duoc bo trong")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mat khau tu 8 den 100 ky tu")]
    [DefaultValue("matkhau123")]
    public string MatKhau { get; set; } = string.Empty;
}

