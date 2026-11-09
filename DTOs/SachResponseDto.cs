namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Du lieu API tra ra ngoai.
///
/// CO MaSach        -> client can biet de goi cac thao tac khac
/// CO DaKiemDuyet   -> client XEM duoc, nhung KHONG SUA duoc
/// KHONG CO GiaNhap -> thong tin kinh doanh noi bo
///
/// Khac biet giua truong "xem duoc" va truong "sua duoc" la diem cot loi cua thiet ke DTO.
/// </summary>
public class SachResponseDto
{
    public int MaSach { get; set; }
    public string TenSach { get; set; } = string.Empty;
    public int NamXuatBan { get; set; }
    public bool ConSan { get; set; }
    public bool DaKiemDuyet { get; set; }

    /// <summary>    
    /// Chi lay TRUONG CAN DUNG, khong dua ca doi tuong TacGia vao DTO
    /// -> khong co vong lap tham chieu.
    /// </summary>
    public string? TenTacGia { get; set; }

    /// <summary>
    /// LAB 7 - Dia chi cong khai cua anh bia, vi du /uploads/3f2c...jpg.
    /// Client ghep voi dia chi may chu de mo anh: https://localhost:7228/uploads/3f2c...jpg
    /// </summary>
    public string? DuongDanAnhBia { get; set; }

    /// <summary>LAB 7 - Ten goc cua anh, chi dung de hien thi.</summary>
    public string? TenAnhGoc { get; set; }
}
