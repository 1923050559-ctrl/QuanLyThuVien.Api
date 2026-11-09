using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Nguoi dung tu DOI mat khau CUA MINH.
/// KHONG co ma tai khoan: server doc tu token, client khong chi dinh duoc "doi mat khau cua ai".
/// </summary>
public class DoiMatKhauDto
{
    [Required(ErrorMessage = "Mat khau cu khong duoc bo trong")]
    [DefaultValue("matkhau123")]
    public string MatKhauCu { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mat khau moi khong duoc bo trong")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mat khau tu 8 den 100 ky tu")]
    [DefaultValue("matkhaumoi456")]
    public string MatKhauMoi { get; set; } = string.Empty;
}
