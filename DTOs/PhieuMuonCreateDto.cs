using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Du lieu client gui len khi MUON SACH. Giao trinh 6.4.3.
///
/// Client CHI gui ma sach va ten nguoi muon.
/// NgayMuon, HanTra, NgayTra do SERVER quyet dinh -> khong co trong DTO nay
/// </summary>
public class PhieuMuonCreateDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Ma sach khong hop le")]
    [DefaultValue(2)]
    public int MaSach { get; set; }

    [Required(ErrorMessage = "Ten nguoi muon khong duoc bo trong")]
    [MaxLength(100, ErrorMessage = "Ten nguoi muon toi da 100 ky tu")]
    [DefaultValue("Nguyen Van B")]
    public string TenNguoiMuon { get; set; } = string.Empty;
}
