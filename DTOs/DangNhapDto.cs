using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// LAB 8 - Du lieu client gui len khi DANG NHAP. Giao trinh 8.6, Phu luc 8.A.1.
/// Thieu truong nao thi [ApiController] tu tra 400 truoc khi vao action.
/// </summary>
public class DangNhapDto
{
    [Required(ErrorMessage = "Ten dang nhap khong duoc bo trong")]
    [DefaultValue("admin")]
    public string TenDangNhap { get; set; } = "";

    [Required(ErrorMessage = "Mat khau khong duoc bo trong")]
    [DefaultValue("admin123")]
    public string MatKhau { get; set; } = "";
}
