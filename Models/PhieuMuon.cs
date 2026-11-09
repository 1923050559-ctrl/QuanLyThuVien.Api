using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Api.Models;

/// <summary>
/// THUC THE GIAO DICH - ghi lai viec phat sinh theo thoi gian.
/// </summary>
public class PhieuMuon
{
    [Key]
    public int MaPhieu { get; set; }

    [Required, MaxLength(100)]
    public string TenNguoiMuon { get; set; } = string.Empty;

    /// <summary>
    /// SQLite KHONG co kieu ngay gio rieng.
    /// EF Core luu duoi dang CHUOI VAN BAN.
    /// </summary>
    public DateTime NgayMuon { get; set; }

    /// <summary>
    /// Dau hoi [?]: phieu chua tra thi truong nay RONG.
    /// </summary>
    public DateTime? NgayTra { get; set; }

    /// <summary>    
    /// Dau hoi [?]: cac phieu muon CU trong app.db chua co gia tri.
    /// Khai bao DateTime co dau [?] thi migration dien 0001-01-01 cho phieu cu.
    /// </summary>
    public DateTime? HanTra { get; set; }
        
    /// <summary>
    /// LAB 9 - CHU SO HUU cua phieu: ma tai khoan da LAP phieu (Giao trinh 9.1.4).
    /// Server tu gan tu MA THONG BAO luc muon sach - client KHONG gui len.
    /// Dau hoi [?]: phieu CU (truoc Lab 9) chua co chu so huu.
    /// Chua khai bao khoa ngoai toi TaiKhoan de migration chi la mot dong AddColumn;
    /// he thong that nen khai bao khoa ngoai.
    /// </summary>
    public int? MaTaiKhoan { get; set; }

    [ForeignKey(nameof(Sach))]
    public int MaSach { get; set; }
        
    public Sach? Sach { get; set; }
}
