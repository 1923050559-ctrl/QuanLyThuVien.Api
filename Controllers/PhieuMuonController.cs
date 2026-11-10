using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien.Api.Data;
using QuanLyThuVien.Api.DTOs;
using QuanLyThuVien.Api.Models;

namespace QuanLyThuVien.Api.Controllers;

/// <summary>
/// Quan ly PHIEU MUON - thuc the GIAO DICH. CRUD DAY DU + nghiep vu muon/tra.
///
///   GET    /api/phieumuon          Danh sach        Admin: tat ca | User: chi phieu CUA MINH
///   GET    /api/phieumuon/{id}     Chi tiet         Admin: moi phieu | User: chi phieu cua minh
///   POST   /api/phieumuon          Muon sach        Dang nhap; chu so huu = nguoi goi (doc tu token)
///   PUT    /api/phieumuon/{id}     Cap nhat         Admin: moi phieu | User: chi phieu cua minh
///   POST   /api/phieumuon/{id}/tra Tra sach         Admin: moi phieu | User: chi phieu cua minh
///   DELETE /api/phieumuon/{id}     Xoa              CHI Admin (phan quyen theo VAI TRO)
///
/// Thao tac tren phieu muon tuan theo QUY TAC NGHIEP VU, va thuong thay doi NHIEU BANG:
///   - Muon sach : them PhieuMuon + sua Sach.ConSan = false
///   - Tra sach  : sua PhieuMuon.NgayTra + sua Sach.ConSan = true
///   - Xoa phieu chua tra: xoa PhieuMuon + tra Sach.ConSan = true
/// Moi thao tac chi goi SaveChangesAsync MOT lan -> ghi trong cung mot giao dich.
///
/// LAB 8: [Authorize] tren CA LOP - phieu muon chua TEN NGUOI MUON (du lieu ca nhan)
/// nen ca thao tac DOC cung phai dang nhap.
///
/// LAB 9 - HAI LOAI PHAN QUYEN trong cung mot controller (Giao trinh 9.1.4):
///   1. Theo VAI TRO   : [Authorize(Roles = "Admin")] tren Delete.
///   2. Theo CHU SO HUU: kiem tra TRONG action - [Authorize] chi hoi "da dang nhap chua?",
///      khong biet phieu nay co phai cua nguoi goi hay khong.
///
/// Danh tinh nguoi goi doc TU MA THONG BAO (User.FindFirst), TUYET DOI khong doc tu tham so
/// hay noi dung yeu cau: client sua duoc tham so, nhung khong sua duoc token vi chu ky se hong.
/// </summary>
[Authorize]
[ApiController]
[Route("api/phieumuon")]
public class PhieuMuonController : ControllerBase
{
    /// <summary>So ngay toi da duoc giu sach, tinh tu ngay muon.</summary>
    private const int SoNgayMuonToiDa = 7;

    /// <summary>Han tra sau khi gia han khong duoc qua so ngay nay ke tu ngay muon.</summary>
    private const int SoNgayGiaHanToiDa = 30;

    private readonly AppDbContext _context;
    private readonly ILogger<PhieuMuonController> _logger;

    public PhieuMuonController(AppDbContext context, ILogger<PhieuMuonController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // =====================================================================
    //  LAB 9 - DOC DANH TINH TU MA THONG BAO
    //  Chi dung duoc trong action co [Authorize] (nho [Authorize] tren lop).
    // =====================================================================

    /// <summary>Ma tai khoan cua nguoi dang goi, lay tu claim NameIdentifier trong token.</summary>
    private int MaNguoiGoi =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    /// <summary>Nguoi goi co vai tro Admin khong - lay tu claim Role trong token.</summary>
    private bool LaAdmin => User.IsInRole("Admin");

    /// <summary>Admin thao tac duoc moi phieu; User chi thao tac duoc phieu CUA MINH.</summary>
    private bool CoQuyenTren(PhieuMuon phieu) =>
        LaAdmin || phieu.MaTaiKhoan == MaNguoiGoi;

    // =====================================================================
    //  READ
    // =====================================================================

    /// <summary>Lay danh sach phieu muon kem ten sach, moi nhat truoc.</summary>
    /// <remarks>
    /// Admin thay TOAN BO phieu. User chi thay phieu CUA MINH.
    /// Cung mot endpoint, cung vai tro duoc phep goi, nhung DU LIEU TRA VE khac nhau:
    /// [Authorize] khong loc duoc du lieu - viec loc phai viet bang ma trong action.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        IQueryable<PhieuMuon> truyVan = _context.PhieuMuon.Include(p => p.Sach);

        // LOC THEO CHU SO HUU: bien cuc bo de EF dich duoc thanh tham so SQL
        if (!LaAdmin)
        {
            var maNguoiGoi = MaNguoiGoi;
            truyVan = truyVan.Where(p => p.MaTaiKhoan == maNguoiGoi);
        }

        var ds = await truyVan
            .OrderByDescending(p => p.NgayMuon)
            .ThenByDescending(p => p.MaPhieu)
            .ToListAsync();                 // ToListAsync o BUOC CUOI

        return Ok(ds.Select(ToDto).ToList());
    }

    /// <summary>Lay mot phieu muon theo ma.</summary>
    /// <remarks>
    /// User goi phieu CUA NGUOI KHAC nhan 404, KHONG phai 403: tra 403 la xac nhan
    /// phieu do co ton tai, con 404 thi khong tiet lo gi (Giao trinh 9.1.4).
    /// </remarks>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var phieu = await _context.PhieuMuon
            .Include(p => p.Sach)
            .FirstOrDefaultAsync(p => p.MaPhieu == id);
        if (phieu == null) return NotFound();

        // KIEM TRA CHU SO HUU - buoc rat hay bi quen. Thieu buoc nay: dang nhap bang tai khoan
        // cua minh roi chi can DOI SO tren dia chi la xem duoc phieu cua nguoi khac.
        if (!CoQuyenTren(phieu)) return NotFound();

        return Ok(ToDto(phieu));
    }

    // =====================================================================
    //  CREATE - MUON SACH
    // =====================================================================

    /// <summary>Muon sach: tao phieu muon moi.</summary>
    /// <remarks>
    /// Sach phai ton tai va dang con san. Han tra = ngay muon + 7 ngay.
    /// Sach duoc chuyen sang ConSan = false trong CUNG mot lan luu.
    /// Chu so huu phieu la TAI KHOAN DANG GOI, doc tu token.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Muon([FromBody] PhieuMuonCreateDto dto)
    {
        var sach = await _context.Sach.FindAsync(dto.MaSach);
        if (sach == null)
        {
            ModelState.AddModelError(nameof(dto.MaSach), "Sach khong ton tai");
            return ValidationProblem(ModelState);
        }
        if (!sach.ConSan)
        {
            ModelState.AddModelError(nameof(dto.MaSach), "Sach dang duoc muon");
            return ValidationProblem(ModelState);
        }

        // Ngay muon, han tra va CHU SO HUU do SERVER quyet dinh - client khong gui len.
        // DTO khong co MaTaiKhoan nen gui kem cung vo tac dung (chan over-posting).
        var ngayMuon = DateTime.Now;
        var phieu = new PhieuMuon
        {
            MaSach = dto.MaSach,
            TenNguoiMuon = dto.TenNguoiMuon,
            NgayMuon = ngayMuon,
            HanTra = ngayMuon.AddDays(SoNgayMuonToiDa),
            MaTaiKhoan = MaNguoiGoi
        };

        _context.PhieuMuon.Add(phieu);      // thay doi 1: bang PhieuMuon
        sach.ConSan = false;                // thay doi 2: bang Sach (dang duoc theo doi)

        // MOT lan luu cho CA HAI thay doi -> cung thanh cong hoac cung khong.
        await _context.SaveChangesAsync();

        // Ghi MA, khong ghi ten nguoi muon (du lieu ca nhan) vao log.
        _logger.LogInformation(
            "Tai khoan {MaTaiKhoan} muon sach {MaSach}, tao phieu {MaPhieu}",
            phieu.MaTaiKhoan, phieu.MaSach, phieu.MaPhieu);

        // phieu.Sach tro toi chinh doi tuong sach dang duoc theo doi,
        // nen ToDto lay duoc TenSach ma khong can Include.
        return CreatedAtAction(nameof(GetById),
            new { id = phieu.MaPhieu }, ToDto(phieu));
    }

    // =====================================================================
    //  UPDATE
    // =====================================================================

    /// <summary>Cap nhat phieu muon: doi ten nguoi muon, gia han han tra.</summary>
    /// <remarks>
    /// Chi sua duoc phieu CHUA TRA. Han tra moi phai sau ngay muon va khong qua 30 ngay ke tu ngay muon.
    /// User sua phieu cua nguoi khac nhan 403; Admin sua duoc moi phieu.
    /// </remarks>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PhieuMuonUpdateDto dto)
    {
        var phieu = await _context.PhieuMuon
            .Include(p => p.Sach)
            .FirstOrDefaultAsync(p => p.MaPhieu == id);
        if (phieu == null) return NotFound();

        // THU TU KIEM TRA: ton tai (404) -> quyen so huu (403) -> quy tac nghiep vu (400).
        // Kiem tra quyen TRUOC nghiep vu: khong de nguoi la biet phieu cua nguoi khac dang o trang thai nao.
        // LAB 9 - TODO 2 (Buoc 4): PHAN QUYEN THEO CHU SO HUU (Giao trinh 9.1.4)
        // Doc danh tinh TU MA THONG BAO da duoc xac thuc
        var maClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(maClaim, out var maNguoiGoi)) return Forbid();

        var laAdmin = User.IsInRole("Admin");

        // Admin sua moi phieu; User chi sua phieu CUA MINH
        if (!laAdmin && phieu.MaTaiKhoan != maNguoiGoi)
        {
            return Forbid(); // -> 403
        }

        if (phieu.NgayTra != null)
        {
            ModelState.AddModelError(nameof(id), "Phieu nay da tra sach, khong the sua");
            return ValidationProblem(ModelState);
        }

        if (dto.HanTra != null)
        {
            var hanToiDa = phieu.NgayMuon.AddDays(SoNgayGiaHanToiDa);
            if (dto.HanTra <= phieu.NgayMuon || dto.HanTra > hanToiDa)
            {
                ModelState.AddModelError(nameof(dto.HanTra),
                    $"Han tra phai sau ngay muon va khong qua {SoNgayGiaHanToiDa} ngay ke tu ngay muon");
                return ValidationProblem(ModelState);
            }
            phieu.HanTra = dto.HanTra;
        }

        phieu.TenNguoiMuon = dto.TenNguoiMuon;
        // KHONG can goi _context.PhieuMuon.Update(phieu) - Giao trinh 6.5.4

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Tai khoan {MaTaiKhoan} cap nhat phieu {MaPhieu}", MaNguoiGoi, phieu.MaPhieu);

        return Ok(ToDto(phieu));
    }

    /// <summary>Tra sach: ghi ngay tra cho phieu muon.</summary>
    /// <remarks>
    /// Dung POST len duong dan con /tra vi day la HANH DONG doi trang thai,
    /// khong phai thay the toan bo tai nguyen. Goi lan hai se bi tu choi.
    /// User chi tra duoc phieu CUA MINH (403); Admin tra duoc moi phieu.
    /// </remarks>
    [HttpPost("{id:int}/tra")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TraSach([FromRoute] int id)
    {
        // Include de SUA bang lien quan (Sach.ConSan) trong cung mot lan luu.
        var phieu = await _context.PhieuMuon
            .Include(p => p.Sach)
            .FirstOrDefaultAsync(p => p.MaPhieu == id);
        if (phieu == null) return NotFound();

        if (!CoQuyenTren(phieu)) return Forbid();   // LAB 9 - chu so huu

        if (phieu.NgayTra != null)
        {
            ModelState.AddModelError(nameof(id), "Phieu nay da tra sach");
            return ValidationProblem(ModelState);
        }

        phieu.NgayTra = DateTime.Now;
        phieu.Sach!.ConSan = true;   // khoa ngoai bat buoc + Include -> Sach co

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Tai khoan {MaTaiKhoan} tra sach, phieu {MaPhieu}", MaNguoiGoi, phieu.MaPhieu);

        return Ok(ToDto(phieu));
    }

    // =====================================================================
    //  DELETE
    // =====================================================================

    /// <summary>Xoa mot phieu muon. CHI ADMIN duoc phep.</summary>
    /// <remarks>
    /// Phieu muon la du lieu LICH SU - he thong that thuong khong cho xoa giao dich da phat sinh.
    /// O day cho Admin xoa de minh hoa phan quyen theo VAI TRO: User nhan 403, khong token nhan 401.
    /// Xoa phieu CHUA TRA thi sach duoc tra ve ConSan = true trong cung lan luu,
    /// neu khong sach se bi "muon vinh vien" boi mot phieu da khong con.
    /// </remarks>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var phieu = await _context.PhieuMuon
            .Include(p => p.Sach)
            .FirstOrDefaultAsync(p => p.MaPhieu == id);
        if (phieu == null) return NotFound();

        if (phieu.NgayTra == null && phieu.Sach != null)
        {
            phieu.Sach.ConSan = true;       // thay doi 1: bang Sach
        }

        _context.PhieuMuon.Remove(phieu);   // thay doi 2: bang PhieuMuon
        await _context.SaveChangesAsync();  // MOT lan luu cho ca hai

        _logger.LogInformation(
            "Admin {MaTaiKhoan} xoa phieu {MaPhieu}", MaNguoiGoi, id);

        return NoContent();
    }

    private static PhieuMuonResponseDto ToDto(PhieuMuon p) => new()
    {
        MaPhieu = p.MaPhieu,
        TenNguoiMuon = p.TenNguoiMuon,
        MaSach = p.MaSach,
        MaTaiKhoan = p.MaTaiKhoan,
        TenSach = p.Sach?.TenSach,
        NgayMuon = p.NgayMuon,
        HanTra = p.HanTra,
        NgayTra = p.NgayTra
    };
}
