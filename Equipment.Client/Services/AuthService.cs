using Equipment.Client.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.JSInterop;

namespace Equipment.Client.Services
{
    // Сервис авторизации — отвечает за вход, выход и хранение токена
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public string? Token { get; private set; }      // Текущий JWT токен
        public string? UserRole { get; private set; }    // Роль пользователя (Admin/Manager/User)
        public string? UserLogin { get; private set; }   // Логин пользователя
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token); // Авторизован ли
        public bool IsAdmin => UserRole == "Admin";      // Является ли администратором
        public bool IsManager => UserRole == "Manager";  // Является ли менеджером
        public bool CanViewDetails => IsAdmin || IsManager; // Может ли просматривать детали

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        // Вход в систему — отправляет логин/пароль на API, получает токен
        public async Task<bool> LoginAsync(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

                if (!response.IsSuccessStatusCode)
                    return false;

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (result == null || string.IsNullOrWhiteSpace(result.Token))
                    return false;

                Token = result.Token;
                ParseToken(Token); // Извлекаем роль из токена

                // Сохраняем токен в LocalStorage
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "token", Token);

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Загрузка сессии из LocalStorage (при обновлении страницы)
        public async Task LoadSessionAsync()
        {
            try
            {
                Token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "token");

                if (!string.IsNullOrWhiteSpace(Token))
                {
                    ParseToken(Token);
                }
            }
            catch
            {
                Token = null;
                UserRole = null;
                UserLogin = null;
            }
        }

        // Выход из системы — удаляем токен из памяти и LocalStorage
        public async Task LogoutAsync()
        {
            Token = null;
            UserRole = null;
            UserLogin = null;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "token");
        }

        // Извлекаем роль и логин из JWT токена (без обращения к серверу)
        private void ParseToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                UserRole = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role
                    || c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
                UserLogin = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name
                    || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            }
            catch
            {
                UserRole = null;
                UserLogin = null;
            }
        }
    }
}
