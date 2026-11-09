namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Du lieu phieu muon API tra ra ngoai. Giao trinh 6.4.3.
/// DTO PHANG: chi lay TenSach tu bang Sach, khong dua ca doi tuong Sach vao.
/// </summary>
public class PhieuMuonResponseDto
{
    public int MaPhieu { get; set; }
    public string TenNguoiMuon { get; set; } = string.Empty;
    public int MaSach { get; set; }

    /// <summary>LAB 9 - ma tai khoan chu so huu phieu (rong voi phieu cu).</summary>
    public int? MaTaiKhoan { get; set; }

    /// <summary>Lay tu bang Sach qua Include.</summary>
    public string? TenSach { get; set; }

    public DateTime NgayMuon { get; set; }
    public DateTime? HanTra { get; set; }
    public DateTime? NgayTra { get; set; }
}
