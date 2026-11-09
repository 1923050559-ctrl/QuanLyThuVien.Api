
using System.Text.Json;

namespace QuanLyThuVien.Api.Middleware
{
    public class XuLyLoiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<XuLyLoiMiddleware> _logger;
        public XuLyLoiMiddleware(RequestDelegate next, ILogger<XuLyLoiMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted) throw;
                var maSuCo = Guid.NewGuid().ToString("N")[..8];
                _logger.LogError(ex, "Su co {MaSuCo} tai {Method} {Path}",
                    maSuCo, context.Request.Method, context.Request.Path);

                context.Response.Clear();
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json; charset=utf-8";

                var phanHoi = new
                {
                    thongBao = "Da co loi xay ra, vui long thu lai",
                    maSuCo = maSuCo
                };
                await context.Response.WriteAsJsonAsync(phanHoi);
            }
        }
    }
}
