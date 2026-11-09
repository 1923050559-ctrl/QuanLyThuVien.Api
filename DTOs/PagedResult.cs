namespace QuanLyThuVien.Api.DTOs;

/// <summary>
/// Boc ket qua phan trang.
/// Tra ve mang thuan thi client khong biet con bao nhieu trang nua.
/// Lop nay bo sung thong tin dieu huong.
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    /// <summary>
    /// Tong so trang. Cong thuc:
    ///     (int)Math.Ceiling(TotalCount / (double)PageSize)
    /// </summary>
    public int TotalPages { get; set; }
}
