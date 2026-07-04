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
    
    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new Role { Name = "User" },
            new Role { Name = "Manager" },
            new Role { Name = "Admin" }
        );
        context.SaveChanges();
    }

    if (!context.Users.Any())
    {
        var userRole = context.Roles.First(r => r.Name == "User");
        var managerRole = context.Roles.First(r => r.Name == "Manager");
        var adminRole = context.Roles.First(r => r.Name == "Admin");

        context.Users.AddRange(
            new User { Login = "user", Password = "123", Role_id = userRole.Id },
            new User { Login = "manager", Password = "123", Role_id = managerRole.Id },
            new User { Login = "admin", Password = "123", Role_id = adminRole.Id }
        );
        context.SaveChanges();
    }

    if (!context.EquipmentTypes.Any())
    {
        context.EquipmentTypes.AddRange(
            new EquipmentType { Name = "Компьютер" },
            new EquipmentType { Name = "Принтер" },
            new EquipmentType { Name = "Сервер" },
            new EquipmentType { Name = "Монитор" }
        );
        context.SaveChanges();
    }

    if (!context.EquipmentStatuses.Any())
    {
        context.EquipmentStatuses.AddRange(
            new EquipmentStatus { Name = "В работе" },
            new EquipmentStatus { Name = "В ремонте" },
            new EquipmentStatus { Name = "Списано" }
        );
        context.SaveChanges();
    }

    if (!context.Employees.Any())
    {
        context.Employees.AddRange(
            new Employee { FullName = "Иванов И.И." },
            new Employee { FullName = "Петров П.П." },
            new Employee { FullName = "Сидоров С.С." }
        );
        context.SaveChanges();
    }

    if (!context.Equipment.Any())
    {
        var typeComputer = context.EquipmentTypes.First(t => t.Name == "Компьютер");
        var typePrinter = context.EquipmentTypes.First(t => t.Name == "Принтер");

        var statusWorking = context.EquipmentStatuses.First(s => s.Name == "В работе");
        var statusRepair = context.EquipmentStatuses.First(s => s.Name == "В ремонте");

        var empIvanov = context.Employees.First(e => e.FullName == "Иванов И.И.");
        var empPetrov = context.Employees.First(e => e.FullName == "Петров П.П.");

        context.Equipment.AddRange(
            new EquipmentItem 
            { 
                InventoryNumber = "INV001", 
                Name = "Laptop Dell", 
                EquipmentTypeId = typeComputer.Id, 
                SerialNumber = "SN123", 
                EquipmentStatusId = statusWorking.Id, 
                EmployeeId = empIvanov.Id 
            },
            new EquipmentItem 
            { 
                InventoryNumber = "INV002", 
                Name = "Printer HP", 
                EquipmentTypeId = typePrinter.Id, 
                SerialNumber = "SN456", 
                EquipmentStatusId = statusRepair.Id, 
                EmployeeId = empPetrov.Id 
            }
        );
        context.SaveChanges();
    }
}

app.MapControllers();

app.Run();
