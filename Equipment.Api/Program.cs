using Equipment.Api.Data;
using Equipment.Api.Models;
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


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // context.Database.Migrate(); // migration might be already applied
    if (!context.Users.Any())
    {
        context.Users.AddRange(
            new User { Login = "user", Password = "123", Role_id = 1 },
            new User { Login = "manager", Password = "123", Role_id = 2 },
            new User { Login = "admin", Password = "123", Role_id = 3 }
        );
        context.SaveChanges();
    }
    if (!context.Equipment.Any())
    {
        context.Equipment.AddRange(
            new EquipmentItem { InventoryNumber = "INV001", Name = "Laptop Dell", Type = "Computer", SerialNumber = "SN123", Status = "Работает", ResponsiblePerson = "Иванов И.И." },
            new EquipmentItem { InventoryNumber = "INV002", Name = "Printer HP", Type = "Printer", SerialNumber = "SN456", Status = "В ремонте", ResponsiblePerson = "Петров П.П." }
        );
        context.SaveChanges();
    }
}

app.MapControllers();

app.Run();
