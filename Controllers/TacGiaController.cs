using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien.Api.Data;
using QuanLyThuVien.Api.DTOs;
using QuanLyThuVien.Api.Models;

namespace QuanLyThuVien.Api.Controllers;

/// <summary>
/// Quan ly tai nguyen TAC GIA - thuc the CHA trong quan he mot - nhieu voi Sach.
/// CRUD DAY DU, bao ve theo cung bang quyet dinh voi SachController (Giao trinh 8.7.2):
///
///   GET    /api/tacgia          Danh sach, loc theo ten, phan trang   CONG KHAI
///   GET    /api/tacgia/{id}     Chi tiet                              CONG KHAI
///   POST   /api/tacgia          Them moi                              [Authorize]
///   PUT    /api/tacgia/{id}     Cap nhat                              [Authorize]
///   DELETE /api/tacgia/{id}     Xoa                                   CHI Admin
///
/// [Authorize] dat tren TUNG ACTION (khong dat tren lop) vi lop nay co ca action cong khai.
/// </summary>
[ApiController]
[Route("api/tacgia")]
public class TacGiaController : ControllerBase
{
    private const int PageSizeMacDinh = 10;
    private const int PageSizeToiDa = 100;

    private readonly AppDbContext _context;
    private readonly ILogger<TacGiaController> _logger;

    public TacGiaController(AppDbContext context, ILogger<TacGiaController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>Lay danh sach tac gia, co loc theo ten va phan trang.</summary>
    /// <param name="tuKhoa">Loc theo ho ten, khong phan biet hoa thuong</param>
    /// <param name="pageNumber">Trang thu may, dem tu 1</param>
    /// <param name="pageSize">So ban ghi moi trang, toi da 100</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? tuKhoa,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = PageSizeMacDinh)
    {
        // BUOC 0 - Chan tham so bat thuong khi phan trang
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = PageSizeMacDinh;
        if (pageSize > PageSizeToiDa) pageSize = PageSizeToiDa;

        // Include de dem SoSach ma khong truy van lai tung tac gia
        IQueryable<TacGia> truyVan = _context.TacGia.Include(t => t.DanhSachSach);

        // BUOC 1 -> LOC
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            truyVan = truyVan.Where(t => EF.Functions.Like(t.HoTen, $"%{tuKhoa}%"));
        }

        // BUOC 2 -> DEM
        var tongSo = await truyVan.CountAsync();

        // BUOC 3 - SAP XEP: sap theo ho ten, them MaTacGia de thu tu on dinh khi trung ten
        truyVan = truyVan.OrderBy(t => t.HoTen).ThenBy(t => t.MaTacGia);

        // BUOC 4 -> CAT TRANG
        var items = await truyVan
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // BUOC 5 - ANH XA sang DTO (chay trong bo nho)
        return Ok(new PagedResult<TacGiaResponseDto>
        {
            Items = items.Select(ToDto).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = tongSo,
            TotalPages = (int)Math.Ceiling(tongSo / (double)pageSize)
        });
    }

    /// <summary>Lay thong tin mot tac gia theo ma.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var tacGia = await _context.TacGia
            .Include(t => t.DanhSachSach)
            .FirstOrDefaultAsync(t => t.MaTacGia == id);
        if (tacGia == null) return NotFound();

        return Ok(ToDto(tacGia));
    }

    /// <summary>Them mot tac gia moi.</summary>
    [HttpPost]
    [Authorize]                     // thao tac GHI: tao moi
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] TacGiaCreateDto dto)
    {
        var tacGia = ToEntity(dto);
        _context.TacGia.Add(tacGia);        // chi danh dau Added, CHUA ghi
        await _context.SaveChangesAsync();  // EF sinh MaTacGia sau khi ghi

        _logger.LogInformation("Da them tac gia {MaTacGia}", tacGia.MaTacGia);

        // Tac gia moi chua co sach -> DanhSachSach rong -> SoSach = 0, khong can Include
        return CreatedAtAction(nameof(GetById),
            new { id = tacGia.MaTacGia }, ToDto(tacGia));
    }

    /// <summary>Cap nhat thong tin mot tac gia.</summary>
    [HttpPut("{id:int}")]
    [Authorize]                     // thao tac GHI: cap nhat
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TacGiaUpdateDto dto)
    {
        var hienCo = await _context.TacGia
            .Include(t => t.DanhSachSach)
            .FirstOrDefaultAsync(t => t.MaTacGia == id);
        if (hienCo == null) return NotFound();

        hienCo.HoTen = dto.HoTen;
        hienCo.TieuSu = dto.TieuSu;
        // KHONG can goi _context.TacGia.Update(hienCo) - Giao trinh 6.5.4

        await _context.SaveChangesAsync();

        _logger.LogInformation("Da cap nhat tac gia {MaTacGia}", id);

        return Ok(ToDto(hienCo));
    }

    /// <summary>Xoa mot tac gia theo ma. CHI ADMIN duoc phep.</summary>
    /// <remarks>
    /// Tac gia CON SACH thi KHONG duoc xoa (400).
    /// Moi quan he Sach - TacGia dung xoa day chuyen (cascade) cua EF Core: xoa tac gia
    /// se xoa theo TOAN BO sach cua tac gia do, va xoa tiep ca phieu muon cua nhung cuon sach do.
    /// Mot thao tac xoa nho keo theo mat du lieu lon - phai chan truoc khi goi Remove.
    /// </remarks>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]    // phan quyen theo VAI TRO - hau qua nang nhat
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var tacGia = await _context.TacGia.FindAsync(id);
        if (tacGia == null) return NotFound();

        // QUY TAC NGHIEP VU: con sach thi khong xoa. Data Annotations khong kiem tra duoc
        // dieu nay vi can truy van database - nhu kiem tra MaTacGia ton tai khi tao sach.
        var soSach = await _context.Sach.CountAsync(s => s.MaTacGia == id);
        if (soSach > 0)
        {
            ModelState.AddModelError(nameof(id),
                $"Tac gia con {soSach} sach, khong the xoa. Xoa hoac chuyen sach truoc");
            return ValidationProblem(ModelState);
        }

        _context.TacGia.Remove(tacGia);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Da xoa tac gia {MaTacGia}", id);

        return NoContent();
    }

    // =====================================================================
    //  ANH XA THU CONG giua Entity va DTO
    // =====================================================================

    private static TacGiaResponseDto ToDto(TacGia t) => new()
    {
        MaTacGia = t.MaTacGia,
        HoTen = t.HoTen,
        TieuSu = t.TieuSu,
        SoSach = t.DanhSachSach.Count      // quen Include thi = 0, khong bao loi
    };

    private static TacGia ToEntity(TacGiaCreateDto dto) => new()
    {
        HoTen = dto.HoTen,
        TieuSu = dto.TieuSu
    };
}
