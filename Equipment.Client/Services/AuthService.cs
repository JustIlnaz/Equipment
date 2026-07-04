using Equipment.Client.Models;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace Equipment.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public string? Token { get; private set; }
        public int? RoleId { get; private set; }
        public string? UserLogin { get; private set; }
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);
        public bool IsAdmin => RoleId == 3;
        public bool IsManager => RoleId == 2;
        public bool IsUser => RoleId == 1;
        public bool CanViewDetails => IsAdmin || IsManager;
        
        public string UserRole 
        { 
            get 
            {
                if (RoleId == 3) return "Admin";
                if (RoleId == 2) return "Manager";
                if (RoleId == 1) return "User";
                return string.Empty;
            }
        }

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

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
                RoleId = result.RoleId;
                UserLogin = result.Login;

                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "token", Token);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "roleId", RoleId.ToString());
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userLogin", UserLogin);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task LoadSessionAsync()
        {
            try
            {
                Token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "token");
                var roleIdStr = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "roleId");
                UserLogin = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userLogin");

                if (!string.IsNullOrWhiteSpace(Token) && int.TryParse(roleIdStr, out var roleId))
                {
                    RoleId = roleId;
                }
                else
                {
                    Token = null;
                    RoleId = null;
                    UserLogin = null;
                }
            }
            catch
            {
                Token = null;
                RoleId = null;
                UserLogin = null;
            }
        }

        public async Task LogoutAsync()
        {
            if (!string.IsNullOrWhiteSpace(Token))
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
                request.Headers.Add("Autorization", Token);
                try { await _httpClient.SendAsync(request); } catch { }
            }

            Token = null;
            RoleId = null;
            UserLogin = null;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "token");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "roleId");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userLogin");
        }
    }
}
