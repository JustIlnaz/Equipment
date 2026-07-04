# 🎓 Пошаговая шпаргалка для экзамена: «Корпоративный каталог оборудования»

Этот гайд поможет вам быстро и последовательно написать весь код с нуля на экзамене, ничего не забыв.

---

## ШАГ 1: Создание проектов и установка пакетов

Создайте пустую папку и откройте в ней терминал.

1. **Создание проектов:**
   ```bash
   dotnet new webapi -n Equipment.Api
   dotnet new blazorwasm -n Equipment.Client
   dotnet new sln -n EquipmentExam
   dotnet sln add Equipment.Api
   dotnet sln add Equipment.Client
   ```

2. **Пакеты для API (`Equipment.Api`):**
   ```bash
   cd Equipment.Api
   dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
   dotnet add package Microsoft.EntityFrameworkCore.Design
   dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
   ```

3. **Пакеты для Клиента (`Equipment.Client`):**
   ```bash
   cd ../Equipment.Client
   dotnet add package System.IdentityModel.Tokens.Jwt
   ```

---

## ШАГ 2: Пишем Сервер (API)

### 2.1. Настройка конфигурации (`appsettings.json`)
Добавьте строку подключения и секреты JWT:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=EquipmentDb;Username=postgres;Password=123"
},
"Jwt": {
  "Key": "SuperSecretKeyForEquipmentExam2024VeryLongKey123456"
}
```

### 2.2. Модели и База Данных (`Models` и `Data`)
1. Создайте папку `Models`.
2. Добавьте `User.cs`: `Id`, `Login`, `Password`, `Role`.
3. Добавьте `EquipmentItem.cs`: `Id`, `InventoryNumber`, `Name`, `Type`, `SerialNumber`, `Status`, `ResponsiblePerson`.
4. Создайте папку `Data`, добавьте `AppDbContext.cs` (наследуется от `DbContext`):
   ```csharp
   public DbSet<EquipmentItem> Equipment { get; set; }
   public DbSet<User> Users { get; set; }
   ```

### 2.3. Настройка `Program.cs`
Порядок важен!
1. **База данных:** `builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(...));`
2. **JWT:**
   ```csharp
   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(opt => { /* настройки валидации, только ключ, без issuer и audience */ });
   builder.Services.AddAuthorization();
   ```
3. **CORS:** Разрешите всё (AllowAnyOrigin, Header, Method).
4. **Middleware (перед `app.MapControllers()`):**
   ```csharp
   app.UseCors();
   app.UseAuthentication();
   app.UseAuthorization();
   ```

### 2.4. Контроллеры (`Controllers`)
1. **AuthController.cs**:
   - Принимает `LoginDto` (Login, Password).
   - Ищет юзера в БД: `_context.Users.FirstOrDefault(...)`.
   - Если найден — генерирует токен (содержит Claims: `Name` и `Role`).
   - Использовать классы `JwtSecurityToken` и `SymmetricSecurityKey`.
   
2. **EquipmentController.cs**:
   - Атрибут над классом: `[Authorize]`
   - `GetAll()` -> `[HttpGet] [Authorize(Roles="Admin,Manager,User")]`
   - `GetById(id)` -> `[HttpGet("{id}")] [Authorize(Roles="Admin,Manager")]`
   - `Create(item)` -> `[HttpPost] [Authorize(Roles="Admin")]`
   - `Update(id, item)` -> `[HttpPut("{id}")] [Authorize(Roles="Admin")]`
   - `Delete(id)` -> `[HttpDelete("{id}")] [Authorize(Roles="Admin")]`

---

## ШАГ 3: Пишем Клиент (Blazor)

### 3.1. Модели
Скопируйте `EquipmentItem.cs` с бэкенда. Создайте `LoginRequest` (Login, Password) и `LoginResponse` (Token).

### 3.2. Сервисы авторизации и API (`Services`)
1. **AuthService.cs**:
   - Внедряем `HttpClient` и `IJSRuntime`.
   - Свойства: `Token`, `UserRole`, `IsAuthenticated`, `IsAdmin`, `IsManager`, `CanViewDetails`.
   - Метод `LoginAsync`: отправляет запрос, получает токен, сохраняет через JS: `await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "token", Token);`
   - Парсинг токена: `JwtSecurityTokenHandler().ReadJwtToken(...)` достает `UserRole`.
   
2. **EquipmentService.cs**:
   - Внедряем `HttpClient` и наш `AuthService`.
   - Метод `SetAuthHeader()`: ставит `DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token)`.
   - CRUD-методы делают HTTP-запросы (`GetFromJsonAsync`, `PostAsJsonAsync` и т.д.).

### 3.3. Настройка `Program.cs`
Добавьте сервисы перед `builder.Build()`:
```csharp
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5126/") });
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EquipmentService>();
```

### 3.4. Интерфейс (`Pages` и `Layout`)
1. **MainLayout.razor**:
   - Шапка (Навигация). Внутри `@if (AuthService.IsAuthenticated)` показываем кнопку "Выход".
   - В `OnAfterRenderAsync` вызываем метод `AuthService.LoadSessionAsync()`, чтобы загрузить токен при обновлении страницы.
   
2. **Login.razor** (`@page "/login"`):
   - Два инпута привязаны к `loginRequest.Login` и `loginRequest.Password`.
   - Кнопка вызывает `AuthService.LoginAsync`. Если успех — `NavigationManager.NavigateTo("/equipment")`.
   
3. **EquipmentList.razor** (`@page "/equipment"`):
   - Проверка прав: `if (!AuthService.IsAuthenticated) NavigateTo("/login");`
   - Таблица с колонками.
   - Колонка "Действия" обернута в `@if (AuthService.CanViewDetails)`.
   - Внутри колонки кнопка "Подробнее". А кнопки "Редактировать" и "Удалить" обернуты в `@if (AuthService.IsAdmin)`.

4. **EquipmentDetails.razor** (`@page "/equipment/{Id:int}"`):
   - Важно: защитить страницу! `if (!AuthService.CanViewDetails) NavigateTo("/equipment");`
   - Показ всех полей `item`.
   
5. **EquipmentForm.razor** (`@page "/equipment/create"` и `@page "/equipment/edit/{Id:int}"`):
   - Форма привязана через `@bind="item.Поле"`.
   - При сохранении проверяем `Id == 0` (создание) или `Id > 0` (редактирование).

---

## 🚀 Порядок действий прямо на экзамене

1. Создать проекты и установить нугеты.
2. Написать модели в API.
3. Поднять контекст базы данных и прописать строку подключения.
4. Создать базу и таблицы руками через DataGrip / DBeaver / pgAdmin, добавить 3 тестовых юзеров.
5. Настроить JWT в `Program.cs`.
6. Сделать AuthController. Проверить через Swagger генерацию токена.
7. Сделать EquipmentController. Проверить его через Swagger, подставляя токен в заголовок.
8. Перейти к Blazor. Сделать модели и `AuthService`.
9. Сделать страницу логина. Проверить, что логин проходит и токен сохраняется в браузере (F12 -> Application -> Local Storage).
10. Сделать `EquipmentService` и `EquipmentList.razor`.
11. Добить `Details` и формы добавления/редактирования. Расставить `@if (AuthService...)` для скрытия кнопок.

Удачи на экзамене!
