namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Du lieu tac gia API tra ra ngoai.
/// DTO PHANG: chi co SoSach (dem), KHONG dua ca danh sach Sach vao -> khong vong lap tham chieu.
/// </summary>
public class TacGiaResponseDto
{
    public int MaTacGia { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string? TieuSu { get; set; }

    /// <summary>So sach cua tac gia - lay tu quan he qua Include.</summary>
    public int SoSach { get; set; }
}
