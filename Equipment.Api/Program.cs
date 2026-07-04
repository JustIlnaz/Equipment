using Equipment.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Подключаем контроллеры
builder.Services.AddControllers();

// Подключаем Swagger для тестирования API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Подключаем базу данных PostgreSQL через Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Настраиваем JWT аутентификацию
// Настройка JWT через свой Middleware

// Настраиваем CORS — разрешаем запросы с клиента Blazor
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// База данных и таблицы должны быть созданы и заполнены вручную

// Включаем CORS
app.UseCors();

// Swagger доступен только в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Порядок middleware важен: сначала аутентификация, потом авторизация


app.MapControllers();

app.Run();
