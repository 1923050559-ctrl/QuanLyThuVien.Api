using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien.Api.Data;
using QuanLyThuVien.Api.DTOs;
using QuanLyThuVien.Api.Helpers;
using QuanLyThuVien.Api.Models;

namespace QuanLyThuVien.Api.Controllers;

/// <summary>
/// LAB 9 (MO RONG) - QUAN LY TAI KHOAN. Toan bo lop CHI DANH CHO ADMIN.
///
///   GET    /api/taikhoan                 Danh sach, loc theo ten va vai tro, phan trang
///   GET    /api/taikhoan/{id}            Chi tiet
///   POST   /api/taikhoan                 Tao tai khoan (Admin chon vai tro)
///   PUT    /api/taikhoan/{id}            Doi VAI TRO
///   PUT    /api/taikhoan/{id}/mat-khau   DAT LAI mat khau (khong can mat khau cu)
///   DELETE /api/taikhoan/{id}            Xoa - co ba rang buoc, xem Delete
///
/// Doi mat khau cua CHINH MINH (can mat khau cu) nam o AuthController:
/// PUT /api/auth/doi-mat-khau, vi moi nguoi dung deu goi duoc, khong chi Admin.
///
/// [Authorize(Roles = "Admin")] dat o cap LOP vi MOI action deu chi danh cho Admin -
/// khac TacGiaController, noi co ca action cong khai nen phai dat tren tung action.
/// </summary>
[ApiController]
[Route("api/taikhoan")]
[Authorize(Roles = "Admin")]
public class TaiKhoanController : ControllerBase
{
    private const int PageSizeMacDinh = 10;
    private const int PageSizeToiDa = 100;

    private readonly AppDbContext _context;
    private readonly ILogger<TaiKhoanController> _logger;

    public TaiKhoanController(AppDbContext context, ILogger<TaiKhoanController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>Ma tai khoan cua Admin dang goi, lay tu claim NameIdentifier trong token.</summary>
    private int MaNguoiGoi =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>
    /// Tai khoan nay co phai la Admin CUOI CUNG khong? Dem trong DATABASE, khong tin token:
    /// token cua mot Admin vua bi ha quyen van con hieu luc den khi het han (Giao trinh 8.3.3).
    /// </summary>
    private async Task<bool> LaAdminCuoiCung(TaiKhoan tk) =>
        tk.VaiTro == "Admin" &&
        await _context.TaiKhoan.CountAsync(t => t.VaiTro == "Admin") <= 1;

    // =====================================================================
    //  READ
    // =====================================================================

    /// <summary>Lay danh sach tai khoan, co loc theo ten, theo vai tro va phan trang.</summary>
    /// <param name="tuKhoa">Loc theo ten dang nhap, khong phan biet hoa thuong</param>
    /// <param name="vaiTro">Admin hoac User; bo trong de lay tat ca</param>
    /// <param name="pageNumber">Trang thu may, dem tu 1</param>
    /// <param name="pageSize">So ban ghi moi trang, toi da 100</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? tuKhoa,
        [FromQuery] string? vaiTro,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = PageSizeMacDinh)
    {
        // BUOC 0 - Chan tham so bat thuong khi phan trang
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = PageSizeMacDinh;
        if (pageSize > PageSizeToiDa) pageSize = PageSizeToiDa;

        IQueryable<TaiKhoan> truyVan = _context.TaiKhoan;

        // BUOC 1 -> LOC
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            truyVan = truyVan.Where(t => EF.Functions.Like(t.TenDangNhap, $"%{tuKhoa}%"));
        }
        if (!string.IsNullOrWhiteSpace(vaiTro))
        {
            truyVan = truyVan.Where(t => t.VaiTro == vaiTro);
        }

        // BUOC 2 -> DEM
        var tongSo = await truyVan.CountAsync();

        // BUOC 3 -> SAP XEP (theo ten, them ma de thu tu on dinh)
        truyVan = truyVan.OrderBy(t => t.TenDangNhap).ThenBy(t => t.MaTaiKhoan);

        // BUOC 4 -> CAT TRANG
        var items = await truyVan
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // BUOC 5 - ANH XA sang DTO: MatKhauBam khong bao gio ra khoi server
        return Ok(new PagedResult<TaiKhoanResponseDto>
        {
            Items = items.Select(ToDto).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = tongSo,
            TotalPages = (int)Math.Ceiling(tongSo / (double)pageSize)
        });
    }

    /// <summary>Lay thong tin mot tai khoan theo ma.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var tk = await _context.TaiKhoan.FindAsync(id);
        if (tk == null) return NotFound();

        return Ok(ToDto(tk));
    }

    // =====================================================================
    //  CREATE
    // =====================================================================

    /// <summary>Tao tai khoan moi. Admin chon vai tro (mac dinh User).</summary>
    /// <remarks>
    /// Ten dang nhap phai DUY NHAT. Kiem tra bang ma de tra thong bao than thien; rang buoc
    /// UNIQUE cua database (migration ThemChiMucTenDangNhap) la lop bao ve cuoi cung khi hai
    /// yeu cau trung ten den cung luc.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] TaiKhoanCreateDto dto)
    {
        // LAB 9 (Mo rong) - TODO 5: chan TRUNG ten dang nhap
        var daTonTai = await _context.TaiKhoan.AnyAsync(t => t.TenDangNhap.ToLower() == dto.TenDangNhap.ToLower());
        if (daTonTai)
        {
            ModelState.AddModelError(nameof(dto.TenDangNhap), "Ten dang nhap da ton tai");
            return ValidationProblem(ModelState);
        }

        var tk = new TaiKhoan
        {
            TenDangNhap = dto.TenDangNhap,
            MatKhauBam = MatKhauHelper.Bam(dto.MatKhau),   // chi luu chuoi bam
            VaiTro = dto.VaiTro
        };

        _context.TaiKhoan.Add(tk);
        await _context.SaveChangesAsync();

        // Ghi ma tai khoan, vai tro; KHONG ghi mat khau
        _logger.LogInformation("Admin {MaNguoiGoi} da tao tai khoan {MaTaiKhoan} vai tro {VaiTro}",
            MaNguoiGoi, tk.MaTaiKhoan, tk.VaiTro);

        return CreatedAtAction(nameof(GetById), new { id = tk.MaTaiKhoan }, ToDto(tk));
    }

    // =====================================================================
    //  UPDATE
    // =====================================================================

    /// <summary>Doi vai tro cua mot tai khoan.</summary>
    /// <remarks>
    /// Khong duoc ha quyen Admin CUOI CUNG (400): he thong khong con ai quan tri.
    /// Vai tro moi chi co hieu luc voi token cap SAU do; token dang dung giu vai tro cu
    /// den khi het han - JWT khong co nut "thu hoi" (Giao trinh 8.3.3).
    /// </remarks>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TaiKhoanUpdateDto dto)
    {
        var tk = await _context.TaiKhoan.FindAsync(id);
        if (tk == null) return NotFound();

        // LAB 9 (Mo rong) - TODO 6: KHONG duoc ha quyen Admin CUOI CUNG
        if (dto.VaiTro != "Admin" && await LaAdminCuoiCung(tk))
        {
            ModelState.AddModelError(nameof(dto.VaiTro), "Khong the ha quyen Admin cuoi cung trong he thong");
            return ValidationProblem(ModelState);
        }

        tk.VaiTro = dto.VaiTro;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Admin {MaNguoiGoi} da doi vai tro tai khoan {MaTaiKhoan} thanh {VaiTro}",
            MaNguoiGoi, id, tk.VaiTro);

        return Ok(ToDto(tk));
    }

    /// <summary>Admin DAT LAI mat khau cho mot tai khoan (khong can mat khau cu).</summary>
    /// <remarks>
    /// Dung khi nguoi dung quen mat khau. Token da cap cho tai khoan do van song den khi het han.
    /// </remarks>
    [HttpPut("{id:int}/mat-khau")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DatLaiMatKhau([FromRoute] int id, [FromBody] DatLaiMatKhauDto dto)
    {
        var tk = await _context.TaiKhoan.FindAsync(id);
        if (tk == null) return NotFound();

        tk.MatKhauBam = MatKhauHelper.Bam(dto.MatKhauMoi);
        await _context.SaveChangesAsync();

        // TUYET DOI khong ghi mat khau moi vao log
        _logger.LogInformation("Admin {MaNguoiGoi} da dat lai mat khau cho tai khoan {MaTaiKhoan}",
            MaNguoiGoi, id);

        return NoContent();
    }

    // =====================================================================
    //  DELETE
    // =====================================================================

    /// <summary>Xoa mot tai khoan. CHI ADMIN, va phai qua ba rang buoc.</summary>
    /// <remarks>
    /// Thu tu kiem tra: ton tai (404) -> rang buoc nghiep vu (400).
    ///   1. Khong tu xoa chinh minh - tranh tu khoa minh ra khoi he thong.
    ///   2. Khong xoa Admin cuoi cung.
    ///   3. Khong xoa tai khoan DA CO PHIEU MUON: phieu muon la du lieu giao dich, can giu de
    ///      doi chieu. Cot PhieuMuon.MaTaiKhoan khong co khoa ngoai nen database KHONG tu chan -
    ///      phai chan bang ma, neu khong phieu se mo coi.
    /// He thong that thuong KHOA tai khoan (them cot trang thai, tu choi dang nhap) thay vi xoa han.
    /// </remarks>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var tk = await _context.TaiKhoan.FindAsync(id);
        if (tk == null) return NotFound();

        // LAB 9 (Mo rong) - TODO 7: BA CHOT CHAN truoc khi xoa
        // Chot 1: Khong tu xoa chinh minh - tranh tu khoa minh ra khoi he thong
        if (id == MaNguoiGoi)
        {
            ModelState.AddModelError(nameof(id), "Khong the tu xoa tai khoan cua chinh minh");
            return ValidationProblem(ModelState);
        }

        // Chot 2: Khong xoa Admin cuoi cung
        if (await LaAdminCuoiCung(tk))
        {
            ModelState.AddModelError(nameof(id), "Khong the xoa Admin cuoi cung trong he thong");
            return ValidationProblem(ModelState);
        }

        // Chot 3: Khong xoa tai khoan DA CO PHIEU MUON
        var coPhieuMuon = await _context.PhieuMuon.AnyAsync(p => p.MaTaiKhoan == id);
        if (coPhieuMuon)
        {
            ModelState.AddModelError(nameof(id), "Khong the xoa tai khoan da co phieu muon");
            return ValidationProblem(ModelState);
        }

        _context.TaiKhoan.Remove(tk);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Admin {MaNguoiGoi} da xoa tai khoan {MaTaiKhoan}", MaNguoiGoi, id);

        return NoContent();
    }

    // =====================================================================
    //  ANH XA THU CONG giua Entity va DTO
    // =====================================================================

    private static TaiKhoanResponseDto ToDto(TaiKhoan t) => new()
    {
        MaTaiKhoan = t.MaTaiKhoan,
        TenDangNhap = t.TenDangNhap,
        VaiTro = t.VaiTro
        // CO Y khong sao chep MatKhauBam
    };
}
