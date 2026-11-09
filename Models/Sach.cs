using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThuVien.Api.Models;

/// <summary>
/// Quan he: TacGia (1) - Sach (nhieu), khoa ngoai MaTacGia nam o Sach.
/// Quan he: Sach (1) - PhieuMuon (nhieu), khoa ngoai MaSach nam o PhieuMuon.
/// </summary>
public class Sach
{
    // Khoa chinh
    [Key] public int MaSach { get; set; }

    [Required, MaxLength(200)]
    public string TenSach { get; set; } = string.Empty;

    public int NamXuatBan { get; set; }

    public bool ConSan { get; set; }

    /// <summary>
    /// Thong tin noi bo. KHONG co trong Response DTO.
    /// </summary>
    public decimal GiaNhap { get; set; }

    /// <summary>
    /// Chi quan tri vien duoc sua. KHONG co trong Create DTO.</summary>
    public bool DaKiemDuyet { get; set; }
        
    [ForeignKey(nameof(TacGia))]
    public int MaTacGia { get; set; }
     
    // Dau hoi [?] vi khi chua nap du lieu lien ket thi thuoc tinh nay rong.
    public TacGia? TacGia { get; set; }

    // ===== THANH PHAN 3: dieu huong NHIEU ban ghi =====
    public List<PhieuMuon> DanhSachPhieuMuon { get; set; } = new();

    // ===== LAB 7: ANH BIA - Giao trinh 7.5.4 va 7.6.2 =====

    /// <summary>
    /// DIA CHI CONG KHAI cua anh bia, vi du: /uploads/3f2c...-a891.jpg
    /// CHI luu DUONG DAN, KHONG luu noi dung tep (Giao trinh 7.6.2).
    /// KHONG luu duong dan vat ly kieu C:\...\wwwroot\... - duong dan do chi ton tai tren may em.
    /// Dau hoi [?]: sach chua co anh bia thi truong nay RONG.
    /// </summary>
    [MaxLength(300)]
    public string? DuongDanAnhBia { get; set; }

    /// <summary>
    /// TEN GOC client gui len - CHI de HIEN THI cho nguoi dung.
    /// KHONG BAO GIO dung ten nay de ghep duong dan luu tep.
    /// </summary>
    [MaxLength(255)]
    public string? TenAnhGoc { get; set; }
}
