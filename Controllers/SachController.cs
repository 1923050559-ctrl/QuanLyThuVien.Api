using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien.Api.Data;
using QuanLyThuVien.Api.DTOs;
using QuanLyThuVien.Api.Helpers;
using QuanLyThuVien.Api.Models;

namespace QuanLyThuVien.Api.Controllers;

/// <summary>
/// Quan ly tai nguyen Sach - LAB 6: BAT DONG BO va INCLUDE.
/// LAB 7: tai anh bia theo QUY TRINH SAU BUOC, thay the anh, don tep khi xoa sach.
/// LAB 8 va 9: BAO VE ENDPOINT theo nghiep vu thu vien thuc te:
///   - GET danh sach, GET chi tiet : CONG KHAI ([AllowAnonymous]) - danh muc sach ai cung xem duoc
///   - POST, PUT, DELETE, tai anh bia: CHI Admin ([Authorize(Roles = "Admin")])
/// [Authorize(Roles = "Admin")] dat tren TUNG ACTION (khong dat tren lop) vi lop nay co ca action cong khai.
///   Nhan ILogger ghi log thao tac GHI (Giao trinh 9.3). Chi ghi MA, khong ghi du lieu ca nhan.
/// </summary>
[ApiController]
[Route("api/sach")]
public class SachController : ControllerBase
{
    private const int PageSizeMacDinh = 10;
    private const int PageSizeToiDa = 100;

    /// <summary>Gioi han THAT cua ung dung: 2 MB. Tang may chu noi len 10 MB trong Program.cs.</summary>
    private const long DungLuongToiDa = 2 * 1024 * 1024;

    /// <summary>DANH SACH CHO PHEP - lop 1.</summary>
    private static readonly string[] PhanMoRongChoPhep =
        { ".jpg", ".jpeg", ".png", ".webp" };

    /// <summary>DANH SACH CHO PHEP - lop 2.</summary>
    private static readonly string[] LoaiChoPhep =
        { "image/jpeg", "image/png", "image/webp" };

    /// <summary>
    /// MO RONG NANG CAO - lop 3. Lab 7 chi BAT BUOC lop 1 va lop 2.
    /// Dat false de tat lop 3 va quay ve dung yeu cau toi thieu cua Lab.
    /// </summary>
    private const bool BatKiemTraLop3 = true;

    /// <summary>Thu muc con trong wwwroot chua anh bia.</summary>
    private const string ThuMucUploads = "uploads";

    private readonly AppDbContext _context;
    private readonly ILogger<SachController> _logger;      // LAB 9 - ghi log (Giao trinh 9.3.2)

    public SachController(AppDbContext context, ILogger<SachController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>Lay danh sach sach co ho tro loc, sap xep va phan trang.</summary>
    /// <param name="tuKhoa">Loc theo ten sach, khong phan biet hoa thuong</param>
    /// <param name="conSan">Loc theo tinh trang con san</param>
    /// <param name="tuNam">Mo rong Lab 4: Loc tu nam xuat ban</param>
    /// <param name="denNam">Mo rong Lab 4: Loc den nam xuat ban</param>
    /// <param name="sortBy">Truong sap xep: tenSach, namXuatBan hoac maTacGia (Mo rong Lab 4)</param>
    /// <param name="sortOrder">Chieu sap xep: asc hoac desc</param>
    /// <param name="pageNumber">Trang thu may, dem tu 1</param>
    /// <param name="pageSize">So ban ghi moi trang, toi da 100</param>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? tuKhoa,
        [FromQuery] bool? conSan,
        [FromQuery] int? tuNam,
        [FromQuery] int? denNam,
        [FromQuery] string? sortBy,
        [FromQuery] string sortOrder = "asc",
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = PageSizeMacDinh)
    {
        // BUOC 0 - Chan tham so bat thuong khi phan trang
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = PageSizeMacDinh;
        if (pageSize > PageSizeToiDa) pageSize = PageSizeToiDa;

        // Include de tra kem ten tac gia        
        IQueryable<Sach> truyVan = _context.Sach.Include(s => s.TacGia);
                
        // BUOC 1 -> LOC
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            truyVan = truyVan.Where(s =>
                EF.Functions.Like(s.TenSach, $"%{tuKhoa}%"));
        }

        if (conSan.HasValue)
        {
            truyVan = truyVan.Where(s => s.ConSan == conSan.Value);
        }

        // Mo rong Lab 4: loc theo khoang nam xuat ban
        if (tuNam.HasValue)
        {
            truyVan = truyVan.Where(s => s.NamXuatBan >= tuNam.Value);
        }

        if (denNam.HasValue)
        {
            truyVan = truyVan.Where(s => s.NamXuatBan <= denNam.Value);
        }

        // BUOC 2 -> DEM        
        var tongSo = await truyVan.CountAsync();

        // BUOC 3 - SAP XEP
        bool tangDan = sortOrder?.ToLower() != "desc";
        truyVan = (sortBy?.ToLower()) switch
        {
            "tensach" => tangDan
                ? truyVan.OrderBy(s => s.TenSach).ThenBy(s => s.MaSach)
                : truyVan.OrderByDescending(s => s.TenSach).ThenBy(s => s.MaSach),
            "namxuatban" => tangDan
                ? truyVan.OrderBy(s => s.NamXuatBan).ThenBy(s => s.MaSach)
                : truyVan.OrderByDescending(s => s.NamXuatBan).ThenBy(s => s.MaSach),
            "matacgia" => tangDan
                ? truyVan.OrderBy(s => s.MaTacGia).ThenBy(s => s.MaSach)
                : truyVan.OrderByDescending(s => s.MaTacGia).ThenBy(s => s.MaSach),
            _ => truyVan.OrderBy(s => s.MaSach)
        };

        // BUOC 4 -> CAT TRANG        
        var items = await truyVan
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        // BUOC 5 - ANH XA sang DTO (chay trong bo nho)
        return Ok(new PagedResult<SachResponseDto>
        {
            Items = items.Select(ToDto).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = tongSo,
            TotalPages = (int)Math.Ceiling(tongSo / (double)pageSize)
        });
    }

    /// <summary>Endpoint thu nghiem gay loi co chu y de kiem chung XuLyLoiMiddleware (Lab 9 - 9d.1).</summary>
    [HttpGet("thu-loi")]
    [AllowAnonymous]
    public IActionResult ThuLoi()
    {
        throw new InvalidOperationException("Loi thu nghiem de kiem chung middleware");
    }

    /// <summary>Lay thong tin mot cuon sach theo ma.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        // Lab 6: FirstOrDefault -> await FirstOrDefaultAsync, kem Include.        
        var sach = await _context.Sach
            .Include(s => s.TacGia)
            .FirstOrDefaultAsync(s => s.MaSach == id);
        if (sach == null) return NotFound();

        return Ok(ToDto(sach));
    }

    /// <summary>Them mot cuon sach moi. CHI ADMIN duoc phep.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]    // CHI Admin duoc them sach
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] SachCreateDto dto)
    {
        // Lab 6: Any -> await FindAsync
        // Vua KIEM TRA tac gia ton tai, vua LAY VE doi tuong de tra TenTacGia.
        var tacGia = await _context.TacGia.FindAsync(dto.MaTacGia);
        if (tacGia == null)
        {
            ModelState.AddModelError(nameof(dto.MaTacGia), "Tac gia khong ton tai");
            return ValidationProblem(ModelState);
        }

        var sach = ToEntity(dto);
        _context.Sach.Add(sach);            // chi danh dau Added, CHUA ghi
        await _context.SaveChangesAsync();  // Lab 6: SaveChanges -> SaveChangesAsync

        // Gan tac gia vua tim de ToDto co TenTacGia ma khong can truy van lai
        sach.TacGia = tacGia;

        _logger.LogInformation("Da them sach {MaSach}", sach.MaSach);       // LAB 9

        return CreatedAtAction(nameof(GetById),
            new { id = sach.MaSach }, ToDto(sach));
    }

    /// <summary>Cap nhat thong tin mot cuon sach. CHI ADMIN duoc phep.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]    // CHI Admin duoc sua sach
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SachUpdateDto dto)
    {
        // Lab 6: FirstOrDefault -> FirstOrDefaultAsync, kem Include de tra TenTacGia
        var hienCo = await _context.Sach
            .Include(s => s.TacGia)
            .FirstOrDefaultAsync(s => s.MaSach == id);
        if (hienCo == null) return NotFound();

        hienCo.TenSach = dto.TenSach;
        hienCo.NamXuatBan = dto.NamXuatBan;
        hienCo.ConSan = dto.ConSan;
        // KHONG can goi _context.Sach.Update(hienCo) - Giao trinh 6.5.4

        await _context.SaveChangesAsync();

        _logger.LogInformation("Da cap nhat sach {MaSach}", id);            // LAB 9

        return Ok(ToDto(hienCo));
    }

    /// <summary>Xoa mot cuon sach theo ma.</summary>
    /// <remarks>
    /// Xoa sach se xoa theo mot phieu muon sach cua cuon sach do.
    /// LAB 9: chi Admin. Khong token -> 401; dang nhap bang docgia -> 403; Admin -> 204.
    /// LAB 7: sach co anh bia thi xoa luon tep anh SAU KHI xoa ban ghi thanh cong (Giao trinh 7.8.3).
    /// </remarks>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var sach = await _context.Sach.FindAsync(id);   // Lab 6: Find -> FindAsync
        if (sach == null) return NotFound();

        // LAB 7: ghi nho duong dan anh TRUOC khi ban ghi bien mat
        var duongDanAnh = sach.DuongDanAnhBia;

        _context.Sach.Remove(sach);         // Remove KHONG can await
        await _context.SaveChangesAsync();

        // LAB 7: CSDL da xoa xong -> moi xoa tep. Neu bo buoc nay, tep anh thanh TEP MO COI.
        // O muc hoc phan: moi sach mot anh rieng, khong dung chung -> xoa ngay la an toan.
        XoaTepTheoDiaChiCongKhai(duongDanAnh);

        _logger.LogInformation("Da xoa sach {MaSach}", id);                 // LAB 9

        return NoContent();
    }

    // =====================================================================
    //  LAB 7 - TAI ANH BIA THEO QUY TRINH SAU BUOC (Giao trinh 7.5, 7.7, 7.8)
    // =====================================================================

    /// <summary>Tai len (hoac thay the) anh bia cho mot cuon sach.</summary>
    /// <remarks>
    /// Gui dang multipart/form-data, truong ten **file**.
    /// Chi nhan .jpg, .jpeg, .png, .webp - toi da 2 MB.
    /// Ten tep tren may chu do SERVER tu sinh; ten goc chi luu de hien thi.
    /// Sach da co anh thi anh cu bi xoa SAU KHI cap nhat CSDL thanh cong.
    /// </remarks>
    /// <param name="id">Ma sach</param>
    /// <param name="file">Tep anh. Ten tham so PHAI trung voi KEY ben Postman: file</param>
    [HttpPost("{id:int}/anh-bia")]
    [Authorize(Roles = "Admin")]    // CHI Admin duoc tai/thay anh bia sach
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(SachResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadAnhBia(
        [FromRoute] int id, IFormFile? file)
    {
        // Ghi chu: khai bao IFormFile? (co dau hoi).
        // Project bat <Nullable>enable</Nullable> nen IFormFile KHONG dau hoi bi coi la BAT BUOC:
        // thieu tep thi [ApiController] tu tra 400 tieng Anh TRUOC KHI vao action,
        // va BUOC 1 ben duoi khong bao gio chay.

        // BUOC 1 - TEP CO TON TAI KHONG
        if (file == null || file.Length == 0)
            return BadRequest(new { thongBao = "Chua chon tep" });

        // BUOC 2 - DUNG LUONG. Length do SERVER tu do -> dang tin.
        if (file.Length > DungLuongToiDa)
        {
            // LAB 9 - Warning: bat thuong nhung chua phai loi. Chi ghi dung luong, khong ghi ten tep.
            _logger.LogWarning("Tu choi tep vuot gioi han, sach {MaSach}, {DungLuong} byte", id, file.Length);
            return BadRequest(new { thongBao = "Tep vuot qua 2 MB" });
        }

        // BUOC 3 - LOAI TEP
        // Lop 1: phan mo rong - chi la LOI KHAI qua ten tep
        var phanMoRong = Path.GetExtension(file.FileName).ToLower();
        if (!PhanMoRongChoPhep.Contains(phanMoRong))
            return BadRequest(new { thongBao = "Phan mo rong khong duoc phep" });

        // Lop 2: ContentType - van chi la LOI KHAI cua client
        if (!LoaiChoPhep.Contains(file.ContentType.ToLower()))
            return BadRequest(new { thongBao = "Loai tep khong duoc phep" });

        // Lop 3 (MO RONG NANG CAO): doc byte dau tep - BANG CHUNG, client khong gia duoc
        if (BatKiemTraLop3 && !await KiemTraDauHieuTep.KhopDinhDangAsync(file, phanMoRong))
        {
            // LAB 9 - tep GIA DANG anh: phan mo rong va ContentType hop le nhung noi dung khong khop
            _logger.LogWarning("Tu choi tep gia dang anh, sach {MaSach}, duoi {PhanMoRong}", id, phanMoRong);
            return BadRequest(new { thongBao = "Noi dung tep khong dung dinh dang anh" });
        }

        // BUOC 4 - TAI NGUYEN LIEN QUAN CO TON TAI KHONG
        // PHAI dat TRUOC buoc ghi tep: sach khong ton tai thi KHONG co tep nao duoc ghi
        // -> khong sinh TEP MO COI (Giao trinh 7.5.2, truong hop kiem thu 5).
        var sach = await _context.Sach
            .Include(s => s.TacGia)
            .FirstOrDefaultAsync(s => s.MaSach == id);
        if (sach == null) return NotFound();

        // BUOC 5 - TU SINH TEN MOI roi ghi tep
        // Ten client gui bi BO QUA HOAN TOAN -> chan duyet duong dan VA chan ghi de trung ten.
        // Phan mo rong lay tu danh sach cho phep (da kiem tra o buoc 3) nen an toan.
        var tenMoi = $"{Guid.NewGuid()}{phanMoRong}";
        var thuMuc = Path.Combine(
            Directory.GetCurrentDirectory(), "wwwroot", ThuMucUploads);
        Directory.CreateDirectory(thuMuc);              // chua co thi tao, co roi thi bo qua
        var duongDanVatLy = Path.Combine(thuMuc, tenMoi);

        // Ghi nho anh CU truoc khi ghi de duong dan (Giao trinh 7.8.2)
        var duongDanCu = sach.DuongDanAnhBia;

        string? duongDanVuaGhi = null;
        try
        {
            // FileMode.CreateNew: neu (rat hiem) trung ten thi bao loi, KHONG ghi de tep da co
            using (var stream = new FileStream(duongDanVatLy, FileMode.CreateNew))
            {
                await file.CopyToAsync(stream);
            }
            duongDanVuaGhi = duongDanVatLy;

            // BUOC 6 - LUU DIA CHI CONG KHAI va TEN GOC vao CSDL
            sach.DuongDanAnhBia = $"/{ThuMucUploads}/{tenMoi}";
            // Path.GetFileName bo phan thu muc (vi du "../../") - ten goc CHI de hien thi
            var tenGoc = Path.GetFileName(file.FileName);
            sach.TenAnhGoc = tenGoc.Length > 255 ? tenGoc[..255] : tenGoc;
            await _context.SaveChangesAsync();
        }
        catch
        {
            // Ghi CSDL that bai -> XOA tep vua ghi, khong de lai TEP MO COI (Giao trinh 7.7.4)
            if (duongDanVuaGhi != null && System.IO.File.Exists(duongDanVuaGhi))
            {
                System.IO.File.Delete(duongDanVuaGhi);
            }
            throw;
        }

        // THAY THE: xoa anh cu SAU KHI CSDL da cap nhat thanh cong (Giao trinh 7.8.2).
        // Xoa TRUOC ma CSDL loi thi mat anh cu trong khi ban ghi van tro toi no.
        XoaTepTheoDiaChiCongKhai(duongDanCu);

        _logger.LogInformation("Da cap nhat anh bia sach {MaSach}", id);    // LAB 9

        return Ok(ToDto(sach));
    }

    /// <summary>
    /// Doi DIA CHI CONG KHAI (/uploads/abc.jpg) sang DUONG DAN VAT LY (.../wwwroot/uploads/abc.jpg)
    /// roi xoa tep neu co. Dung chung cho THAY THE anh va XOA sach.
    /// </summary>
    private static void XoaTepTheoDiaChiCongKhai(string? diaChiCongKhai)
    {
        if (string.IsNullOrEmpty(diaChiCongKhai)) return;

        var duongDanVatLy = Path.Combine(
            Directory.GetCurrentDirectory(), "wwwroot", diaChiCongKhai.TrimStart('/'));

        if (System.IO.File.Exists(duongDanVatLy))
        {
            System.IO.File.Delete(duongDanVatLy);
        }
    }

    // =====================================================================
    //  ANH XA THU CONG giua Entity va DTO
    // =====================================================================

    private static SachResponseDto ToDto(Sach s) => new()
    {
        MaSach = s.MaSach,
        TenSach = s.TenSach,
        NamXuatBan = s.NamXuatBan,
        ConSan = s.ConSan,
        DaKiemDuyet = s.DaKiemDuyet,
        TenTacGia = s.TacGia?.HoTen,     // dau hoi: RONG neu quen Include
        DuongDanAnhBia = s.DuongDanAnhBia,   // LAB 7
        TenAnhGoc = s.TenAnhGoc              // LAB 7
        // KHONG anh xa GiaNhap - thong tin noi bo
    };

    private static Sach ToEntity(SachCreateDto dto) => new()
    {
        TenSach = dto.TenSach,
        NamXuatBan = dto.NamXuatBan,
        ConSan = dto.ConSan,
        MaTacGia = dto.MaTacGia,
        GiaNhap = 0,
        DaKiemDuyet = false
    };
}
