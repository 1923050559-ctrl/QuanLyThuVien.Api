using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuanLyThuVien.Api.Data;
using QuanLyThuVien.Api.DTOs;
using QuanLyThuVien.Api.Helpers;
using QuanLyThuVien.Api.Models;

namespace QuanLyThuVien.Api.Controllers;

/// <summary>
/// LAB 8 - DANG NHAP va CAP MA THONG BAO (JWT)
/// LAB 9 (mo rong) - nguoi dung tu DOI MAT KHAU cua minh.
/// KHONG dat [Authorize] o cap LOP: se khoa luon action dang nhap.
/// Quan ly tai khoan (tao, sua vai tro, dat lai mat khau, xoa) o TaiKhoanController - chi Admin.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;      // LAB 9 - ghi log dang nhap

    public AuthController(AppDbContext context, IConfiguration config, ILogger<AuthController> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }

    /// <summary>Dang nhap, nhan ve ma thong bao (JWT).</summary>
    /// <remarks>
    /// Hai tai khoan nap san: admin / admin123 (Admin), docgia / user123 (User).
    /// Sai ten HOAC sai mat khau deu tra 401 voi CUNG MOT thong bao.
    /// </remarks>
    [HttpPost("dang-nhap")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DangNhap([FromBody] DangNhapDto dto)
    {
        // 1. Tim tai khoan theo ten dang nhap
        var tk = await _context.TaiKhoan
            .FirstOrDefaultAsync(t => t.TenDangNhap == dto.TenDangNhap);

        // 2. Bam mat khau gui len roi SO SANH voi chuoi da luu.
        //    MOT dieu kien, MOT thong bao chung cho CA HAI truong hop sai:
        //    tach ra hai thong bao thi ke tan cong do duoc ten dang nhap nao TON TAI.
        if (tk == null || tk.MatKhauBam != MatKhauHelper.Bam(dto.MatKhau))
        {
            // LAB 9 - LOG co the tach hai truong hop (nguoi quan tri can biet), con PHAN HOI
            // thi van CHUNG CHUNG. Ghi ten dang nhap da thu; TUYET DOI khong ghi mat khau
            // - ke ca mat khau SAI nguoi dung vua go (Giao trinh 9.3.4).
            _logger.LogWarning("Dang nhap that bai cho tai khoan {TenDangNhap}", dto.TenDangNhap);
            return Unauthorized(new
                { thongBao = "Sai ten dang nhap hoac mat khau" });
        }

        // 3. Sinh ma thong bao
        var token = SinhToken(tk);

        // LAB 9 - ghi MA tai khoan, KHONG ghi token: ai doc duoc log se mao danh duoc nguoi dung.
        _logger.LogInformation("Da cap token cho tai khoan {MaTaiKhoan}", tk.MaTaiKhoan);

        return Ok(new { token, tk.TenDangNhap, tk.VaiTro });
    }

    /// <summary>Dang ky tai khoan moi cho doc gia (vai tro mac dinh: User).</summary>
    /// <remarks>
    /// Endpoint cong khai: ai cung goi duoc.
    /// DTO khong chua truong VaiTro -> server luon gan VaiTro = "User",
    /// ngan chan viec tu cap quyen Admin qua over-posting.
    /// </remarks>
    [HttpPost("dang-ky")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DangKy([FromBody] DangKyDto dto)
    {
        // 1. Kiem tra ten dang nhap da ton tai chua
        var daTonTai = await _context.TaiKhoan
            .AnyAsync(t => t.TenDangNhap.ToLower() == dto.TenDangNhap.ToLower());
        if (daTonTai)
        {
            ModelState.AddModelError(nameof(dto.TenDangNhap), "Ten dang nhap da ton tai");
            return ValidationProblem(ModelState);
        }

        // 2. Tao tai khoan moi voi vai tro mac dinh "User"
        var tk = new TaiKhoan
        {
            TenDangNhap = dto.TenDangNhap,
            MatKhauBam = MatKhauHelper.Bam(dto.MatKhau), // Bam mat khau bang SHA256 truoc khi luu
            VaiTro = "User"                             // Co dinh vai tro, tranh over-posting
        };

        _context.TaiKhoan.Add(tk);
        await _context.SaveChangesAsync();

        // Ghi log an toan: ghi ma tai khoan va ten dang nhap, KHONG ghi mat khau
        _logger.LogInformation("Nguoi dung {TenDangNhap} da dang ky tai khoan thanh cong, ma {MaTaiKhoan}",
            tk.TenDangNhap, tk.MaTaiKhoan);

        return StatusCode(StatusCodes.Status201Created, new
        {
            thongBao = "Dang ky tai khoan thanh cong",
            maTaiKhoan = tk.MaTaiKhoan,
            tenDangNhap = tk.TenDangNhap,
            vaiTro = tk.VaiTro
        });
    }

    /// <summary>MO RONG - xem server doc duoc gi tu token dang gui len.</summary>
    /// <remarks>
    /// Khong bat buoc trong Lab 8. Dung de minh hoa: payload cua token duoc doc lai
    /// o phia server trong MOI loi goi co gan token (Giao trinh 8.6). Buoi 9 dung
    /// VaiTro de phan quyen va MaTaiKhoan de kiem tra quyen so huu.
    /// </remarks>
    [HttpGet("thong-tin")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ThongTin()
    {
        return Ok(new
        {
            maTaiKhoan = User.FindFirstValue(ClaimTypes.NameIdentifier),
            tenDangNhap = User.Identity?.Name,
            vaiTro = User.FindFirstValue(ClaimTypes.Role),
            laAdmin = User.IsInRole("Admin")
        });
    }

    /// <summary>LAB 9 (mo rong) - Tu DOI mat khau cua CHINH MINH. Phai nhap mat khau cu.</summary>
    /// <remarks>
    /// Ma tai khoan lay tu TOKEN, DTO khong co truong nay: client khong chi dinh duoc "doi mat khau
    /// cua ai" (cung nguyen tac voi chu so huu phieu muon, Giao trinh 9.1.4).
    /// Doi xong, token dang dung van song den khi het han (JWT khong co nut thu hoi).
    /// </remarks>
    [HttpPut("doi-mat-khau")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DoiMatKhau([FromBody] DoiMatKhauDto dto)
    {
        // LAB 9 (Mo rong) - TODO 4: Tu doi mat khau
        // 1. Doc ma tai khoan TU TOKEN (claim NameIdentifier), KHONG nhan tu client
        var maClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(maClaim, out var maTaiKhoan))
        {
            return Unauthorized();
        }

        // 2. Tim tai khoan bang FindAsync. Khong thay (token con han nhung tai khoan da bi xoa) -> Unauthorized()
        var tk = await _context.TaiKhoan.FindAsync(maTaiKhoan);
        if (tk == null)
        {
            return Unauthorized();
        }

        // 3. Bam dto.MatKhauCu bang MatKhauHelper.Bam roi so voi tk.MatKhauBam
        if (tk.MatKhauBam != MatKhauHelper.Bam(dto.MatKhauCu))
        {
            // LogWarning: chi ghi ma tai khoan, KHONG ghi mat khau
            _logger.LogWarning("Doi mat khau that bai cho tai khoan {MaTaiKhoan}: mat khau cu khong dung", maTaiKhoan);
            ModelState.AddModelError(nameof(dto.MatKhauCu), "Mat khau cu khong chinh xac");
            return ValidationProblem(ModelState);
        }

        // 4. Mat khau moi trung mat khau cu -> loi 400 tren truong MatKhauMoi
        if (dto.MatKhauMoi == dto.MatKhauCu)
        {
            ModelState.AddModelError(nameof(dto.MatKhauMoi), "Mat khau moi khong duoc trung mat khau cu");
            return ValidationProblem(ModelState);
        }

        // 5. Gan tk.MatKhauBam moi, SaveChangesAsync, LogInformation (khong ghi mat khau), tra NoContent()
        tk.MatKhauBam = MatKhauHelper.Bam(dto.MatKhauMoi);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Tai khoan {MaTaiKhoan} da doi mat khau thanh cong", maTaiKhoan);
        return NoContent();
    }

    // =====================================================================
    //  HAM PHU - CAP SAN (Phu luc 8.A.3)
    //  Ham BAM MAT KHAU da chuyen sang Helpers/MatKhauHelper.cs (Lab 9): dang nhap, doi mat khau,
    //  tao tai khoan, dat lai mat khau cung dung MOT ham duy nhat.
    // =====================================================================

    /// <summary>Sinh JWT ky bang HS256 voi khoa trong appsettings.json (muc Jwt).</summary>
    private string SinhToken(TaiKhoan tk)
    {
        // Ba thong tin dua vao PAYLOAD - ai co token cung DOC duoc (Giao trinh 8.2.2).
        // TUYET DOI khong dua mat khau hay du lieu nhay cam vao day.
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, tk.MaTaiKhoan.ToString()),
            new Claim(ClaimTypes.Name, tk.TenDangNhap),
            new Claim(ClaimTypes.Role, tk.VaiTro)      // dung o buoi 9
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_config.GetValue<int>("Jwt:ExpiryHours")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
