using Microsoft.AspNetCore.Authentication.JwtBearer; // Mới thêm
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;                // Mới thêm
using Microsoft.OpenApi.Models;                      // Mới thêm
using System.Text;                                   // Mới thêm
using TaskFlow.Api.Data;
using TaskFlow.Api.Repositories;
using TaskFlow.Api.Repositories.Interfaces;
using TaskFlow.Api.Services;
using TaskFlow.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. CẤU HÌNH KẾT NỐI DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// 2. ĐĂNG KÝ REPOSITORY & SERVICE
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITodoItemService, TodoItemService>();
builder.Services.AddScoped<IAuthService, AuthService>(); // Quan trọng!

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 3. CẤU HÌNH AUTHENTICATION (SOÁT VÉ JWT) - QUAN TRỌNG
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // 1. QUAN TRỌNG NHẤT: Có kiểm tra chữ ký (Con dấu) không?
            // -> Có! (true). Nếu không kiểm tra cái này, ai cũng tự vẽ vé giả được.
            ValidateIssuerSigningKey = true,

            // 2. Lấy mẫu "Con dấu gốc" ở đâu để so sánh?
            // -> Vào file appsettings.json, lấy cái chuỗi "Token" ra,
            //    chuyển nó thành dạng bytes để làm mẫu đối chiếu.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),

            // 3. Có kiểm tra ai là người phát hành (Issuer) không?
            // -> Tạm thời là KHÔNG (false) cho dễ chạy.
            //    (Sau này làm dự án lớn, server A phát hành thì server B mới nhận, lúc đó mới cần true).
            ValidateIssuer = false,

            // 4. Có kiểm tra vé này dành cho ai (Audience) không?
            // -> Tạm thời là KHÔNG (false).
            ValidateAudience = false
        };
    });

// 4. CẤU HÌNH SWAGGER (HIỆN NÚT Ổ KHÓA)
builder.Services.AddSwaggerGen(c =>
{
    // 1. Định nghĩa cái nút "Authorize" (Cái ổ khóa)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token theo dạng: Bearer {token}", // Hướng dẫn sử dụng
        Name = "Authorization", // Tên cái dòng sẽ gửi đi trong Header HTTP
        In = ParameterLocation.Header, // Vị trí gửi (Gửi trong Header)
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer" // Kiểu xác thực
    });

    // 2. Kích hoạt cái nút đó
    // (Nếu không có đoạn này, hiện nút ổ khóa nhưng bấm vào nó không có tác dụng gì cả)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Phải trùng tên với cái "Bearer" định nghĩa ở trên
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

app.UseMiddleware<TaskFlow.Api.Middlewares.ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 5. KÍCH HOẠT BẢO VỆ (Thứ tự cực kỳ quan trọng!)
app.UseAuthentication(); // <--- MỚI: Kiểm tra danh tính (Bạn là ai?)
app.UseAuthorization();  // <--- CŨ: Kiểm tra quyền (Bạn được làm gì?)

app.MapControllers();

app.Run();