namespace QuanLyThuVien.Api.Helpers;

/// <summary>
/// LAB 7 - MO RONG NANG CAO: LOP KIEM TRA THU BA (Giao trinh 7.4.3, Bai tap 7.8).
///
/// Lop 1 (phan mo rong) va lop 2 (ContentType) chi kiem tra LOI KHAI cua client.
/// Lop 3 doc vai byte DAU TEP - noi dung THAT, client khong khai bao duoc.
///
///   Dinh dang   Byte dau tep (hex)                 Ghi chu
///   ---------   --------------------------------   ------------------------------
///   jpg/jpeg    FF D8 FF                           3 byte dau
///   png         89 50 4E 47 0D 0A 1A 0A            8 byte dau, ".PNG...."
///   webp        52 49 46 46 ?? ?? ?? ?? 57 45 42 50 "RIFF" + 4 byte kich thuoc + "WEBP"
///   gif         47 49 46 38                        "GIF8" (tham khao, khong cho phep)
///   pdf         25 50 44 46                        "%PDF" (tham khao, khong cho phep)
///
/// Gioi han: tep DUNG dinh dang nhung duoc dung de khai thac loi thu vien xu ly anh
/// van vuot qua lop nay (Giao trinh - Huong dan giai Bai tap 7.8).
/// </summary>
public static class KiemTraDauHieuTep
{
    private static readonly byte[] DauJpg = { 0xFF, 0xD8, 0xFF };
    private static readonly byte[] DauPng = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
    private static readonly byte[] DauRiff = { 0x52, 0x49, 0x46, 0x46 };   // "RIFF"
    private static readonly byte[] DauWebp = { 0x57, 0x45, 0x42, 0x50 };   // "WEBP"

    /// <summary>
    /// Doc 12 byte dau cua tep va so khop voi dau hieu cua PHAN MO RONG da khai.
    /// Tra ve true neu noi dung that KHOP voi phan mo rong.
    /// </summary>
    /// <param name="file">Tep client gui len</param>
    /// <param name="phanMoRong">Phan mo rong da chuan hoa chu thuong, vi du ".jpg"</param>
    public static async Task<bool> KhopDinhDangAsync(IFormFile file, string phanMoRong)
    {
        var dau = new byte[12];
        int daDoc;

        // OpenReadStream mo luong doc MOI moi lan goi -> khong anh huong CopyToAsync phia sau.
        using (var stream = file.OpenReadStream())
        {
            daDoc = await stream.ReadAtLeastAsync(dau, dau.Length, throwOnEndOfStream: false);
        }

        return phanMoRong switch
        {
            ".jpg" or ".jpeg" => BatDauBang(dau, daDoc, DauJpg, 0),
            ".png"            => BatDauBang(dau, daDoc, DauPng, 0),
            ".webp"           => BatDauBang(dau, daDoc, DauRiff, 0)
                                 && BatDauBang(dau, daDoc, DauWebp, 8),
            _                 => false      // DANH SACH CHO PHEP: khong co trong bang -> tu choi
        };
    }

    private static bool BatDauBang(byte[] dau, int daDoc, byte[] mau, int viTri)
    {
        if (daDoc < viTri + mau.Length) return false;
        for (int i = 0; i < mau.Length; i++)
        {
            if (dau[viTri + i] != mau[i]) return false;
        }
        return true;
    }
}
