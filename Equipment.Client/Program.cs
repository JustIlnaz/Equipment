using Equipment.Client;
using Equipment.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Подключаем LocalStorage через JS Interop (встроено)

// Настраиваем HttpClient с базовым адресом API сервера
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5126/") });

// Регистрируем сервис авторизации
builder.Services.AddScoped<AuthService>();

// Регистрируем сервис для работы с оборудованием
builder.Services.AddScoped<EquipmentService>();

await builder.Build().RunAsync();
