using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.Models;

/// <summary>
/// Thuc the CHA trong quan he mot - nhieu.
///
///   Tren DTO   -> kiem tra du lieu client gui len, tra ve ma 400
///                 -> CO ErrorMessage tieng Viet
///   Tren Entity -> cho EF Core biet cach SINH CAU TRUC COT
///                 -> KHONG can ErrorMessage
///
/// [Required, MaxLength(100)] o day sinh ra cot: HoTen TEXT NOT NULL  (do dai toi da 100)
/// </summary>
public class TacGia
{
    [Key] public int MaTacGia { get; set; }

    [Required, MaxLength(100)]
    public string HoTen { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? TieuSu { get; set; }
    
    public List<Sach> DanhSachSach { get; set; } = new();
}
