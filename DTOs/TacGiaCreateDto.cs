using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Du lieu client gui len khi TAO MOI tac gia.
/// KHONG co MaTacGia (do SERVER sinh) va KHONG co danh sach sach (quan he quan ly o Sach).
/// </summary>
public class TacGiaCreateDto
{
    [Required(ErrorMessage = "Ho ten khong duoc bo trong")]
    [MaxLength(100, ErrorMessage = "Ho ten toi da 100 ky tu")]
    [DefaultValue("Nguyen Van G")]
    public string HoTen { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Tieu su toi da 500 ky tu")]
    [DefaultValue("Tac gia moi")]
    public string? TieuSu { get; set; }
}
