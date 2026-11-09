using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Du lieu Admin gui len khi SUA tai khoan: CHI doi duoc VAI TRO.
/// Ten dang nhap la dinh danh ben vung (xuat hien trong log va token) nen khong cho doi.
/// Mat khau co endpoint rieng (dat lai / doi mat khau), khong di chung voi vai tro.
/// </summary>
public class TaiKhoanUpdateDto
{
    [Required(ErrorMessage = "Vai tro khong duoc bo trong")]
    [RegularExpression("^(Admin|User)$", ErrorMessage = "Vai tro chi nhan Admin hoac User")]
    [DefaultValue("User")]
    public string VaiTro { get; set; } = string.Empty;
}
