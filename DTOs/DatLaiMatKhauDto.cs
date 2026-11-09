using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.DTOs;

/// <summary>Admin DAT LAI mat khau cho tai khoan khac - khong can biet mat khau cu.</summary>
public class DatLaiMatKhauDto
{
    [Required(ErrorMessage = "Mat khau moi khong duoc bo trong")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mat khau tu 8 den 100 ky tu")]
    [DefaultValue("matkhaumoi456")]
    public string MatKhauMoi { get; set; } = string.Empty;
}
