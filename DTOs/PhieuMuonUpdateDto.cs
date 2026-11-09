using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// LAB 9 - Du lieu client gui len khi CAP NHAT phieu muon.
///
/// Chi cho sua hai thu: ten nguoi muon va GIA HAN han tra.
/// KHONG co MaSach, MaTaiKhoan, NgayMuon, NgayTra -> client khong doi duoc
/// sach duoc muon, chu so huu, hay tu ghi ngay tra (da co POST /{id}/tra).
/// </summary>
public class PhieuMuonUpdateDto
{
    [Required(ErrorMessage = "Ten nguoi muon khong duoc bo trong")]
    [MaxLength(100, ErrorMessage = "Ten nguoi muon toi da 100 ky tu")]
    [DefaultValue("Nguyen Van B")]
    public string TenNguoiMuon { get; set; } = string.Empty;

    /// <summary>
    /// Han tra moi (gia han). De trong = giu nguyen han cu.
    /// Quy tac ve khoang ngay hop le kiem tra trong controller.
    /// </summary>
    public DateTime? HanTra { get; set; }
}
