using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Du lieu client gui len khi CAP NHAT.
/// </summary>
public class SachUpdateDto
{
    [Required(ErrorMessage = "Ten sach khong duoc bo trong")]
    [MaxLength(200, ErrorMessage = "Ten sach toi da 200 ky tu")]
    [DefaultValue("Ten sach da sua")]
    public string TenSach { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Nam xuat ban phai tu 1900 den 2100")]
    [DefaultValue(2026)]
    public int NamXuatBan { get; set; }

    [DefaultValue(true)]
    public bool ConSan { get; set; }
}
