using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Du lieu client gui len khi TAO MOI.
///
/// KHONG co MaSach      -> ma dinh danh do SERVER sinh
/// KHONG co GiaNhap     -> thong tin noi bo
/// KHONG co DaKiemDuyet -> chi quan tri vien duoc sua
///
/// Day chinh la cach DTO chan tan cong OVER-POSTING
/// </summary>
public class SachCreateDto
{
    [Required(ErrorMessage = "Ten sach khong duoc bo trong")]
    [MaxLength(200, ErrorMessage = "Ten sach toi da 200 ky tu")]
    [DefaultValue("Sach moi abc")]
    public string TenSach { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Nam xuat ban phai tu 1900 den 2100")]
    [DefaultValue(2026)]
    public int NamXuatBan { get; set; }

    [DefaultValue(true)]
    public bool ConSan { get; set; }

    /// <summary>
    /// CO MaTacGia -> sach bat buoc thuoc ve mot tac gia (khoa ngoai).
    /// Viec tac gia co TON TAI trong database hay khong
    /// thi controller kiem tra, vi Data Annotations khong truy van database duoc.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Ma tac gia khong hop le")]
    [DefaultValue(1)]
    public int MaTacGia { get; set; }
}
