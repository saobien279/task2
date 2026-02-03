using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Services.Interfaces;
using TaskFlow.Api.Services;
using TaskFlow.Api.Repositories.Interfaces;
using TaskFlow.Api.Repositories;
var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký Repository
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>(); // <--- Mới thêm

// Đăng ký Service
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITodoItemService, TodoItemService>();       // <--- Mới thêm

// Thay thế các dòng AddAutoMapper cũ bằng dòng này
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
app.UseMiddleware<TaskFlow.Api.Middlewares.ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
