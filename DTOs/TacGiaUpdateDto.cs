using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.DTOs;

/// <summary>Du lieu client gui len khi CAP NHAT tac gia (thay the ho ten va tieu su).</summary>
public class TacGiaUpdateDto
{
    [Required(ErrorMessage = "Ho ten khong duoc bo trong")]
    [MaxLength(100, ErrorMessage = "Ho ten toi da 100 ky tu")]
    [DefaultValue("Nguyen Van G (da sua)")]
    public string HoTen { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Tieu su toi da 500 ky tu")]
    [DefaultValue("Tieu su da cap nhat")]
    public string? TieuSu { get; set; }
}
