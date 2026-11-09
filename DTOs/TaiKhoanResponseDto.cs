namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Du lieu tai khoan API tra ra ngoai.
/// TUYET DOI khong co MatKhauBam: chuoi bam cung la thong tin nhay cam,
/// tra ra la tao co hoi cho ke tan cong do mat khau ngoai tuyen.
/// </summary>
public class TaiKhoanResponseDto
{
    public int MaTaiKhoan { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public string VaiTro { get; set; } = string.Empty;
}
