// =============================================================
//  Hoc phan Web API - Lab 1 -> 9: PHAN QUYEN, MIDDLEWARE XU LY LOI, GHI LOG
// =============================================================

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuanLyThuVien.Api.Data;
using QuanLyThuVien.Api.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Neu load du lieu mau tu file JSON thi dung AppDbContextJson thay cho AppDbContext.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("WebLocal", policy =>
    {
        // Giao dien HTML co the mo tren may khac; API xac thuc bang Bearer JWT.
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly
        .GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // CHO SWAGGER NHAN TOKEN
    // Sau khi chay, Swagger co nut Authorize: dan token vao, KHONG can go chu Bearer.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Dan token lay tu POST /api/auth/dang-nhap (khong go chu Bearer)"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                    { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Tang MAY CHU: noi len 10 MB - cao hon nguong ung dung (2 MB, kiem tra trong action).
// Neu tang may chu THAP hon tang ung dung, yeu cau bi tu choi TRUOC KHI vao action
// -> sinh vien khong bao gio nhan duoc thong bao 400 "Tep vuot qua 2 MB" cua minh.
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;   // 10 MB
});

// =============================================================
//  DANG KY DICH VU XAC THUC JWT
//  Doan nay duoc CAP SAN. Gia tri doc tu muc "Jwt" trong appsettings.json.
// =============================================================
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                  // kiem tra BEN PHAT HANH
            ValidateAudience = true,                // kiem tra BEN NHAN
            ValidateLifetime = true,                // kiem tra HAN DUNG
            ValidateIssuerSigningKey = true,        // kiem tra CHU KY
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            // Mac dinh .NET cho lech them 5 phut sau khi het han.
            // Dat ve 0 de thu tinh huong het han cho ket qua dung ngay (Giao trinh 8.5.1).
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// =============================================================
//  LAB 9 - TODO 3 (Buoc 5) - MIDDLEWARE XU LY LOI: DAT DAU TIEN, NGAY SAU builder.Build()
//  Middleware chi boc duoc nhung buoc dung SAU no trong chuoi. Dat o dau -> la lop ngoai cung,
//  bat duoc ngoai le tu MOI lop ben trong.
// =============================================================
app.UseMiddleware<XuLyLoiMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("WebLocal");

// Dat TRUOC app.MapControllers().
// Moi tep trong wwwroot la CONG KHAI: ai biet dia chi deu tai duoc (Giao trinh 7.6.3).
app.UseDefaultFiles();
app.UseStaticFiles();

// =============================================================
//  LAB 8 - THU TU BAT BUOC
//  Lan thu BA trong hoc phan thu tu trong Program.cs quyet dinh hanh vi.
//  1. UseAuthentication: doc token, kiem tra chu ky + han dung -> "Anh la ai?"
//  2. UseAuthorization : dung danh tinh vua co de quyet dinh -> "Anh duoc phep lam gi?"
//  Dao nguoc hai dong -> moi endpoint co [Authorize] tra 401 DU token hop le.
// =============================================================
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
