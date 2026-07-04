using Equipment.Client.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Equipment.Client.Services
{
    // Сервис для работы с API оборудования
    public class EquipmentService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public EquipmentService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        // Добавляем custom токен к каждому запросу
        private void SetAuthHeader()
        {
            _httpClient.DefaultRequestHeaders.Remove("Autorization");
            if (!string.IsNullOrWhiteSpace(_authService.Token))
            {
                _httpClient.DefaultRequestHeaders.Add("Autorization", _authService.Token);
            }
        }

        // Получить список всего оборудования
        public async Task<List<EquipmentItem>?> GetAllAsync()
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync("api/equipment");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return null; // Нет авторизации

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<EquipmentItem>>();
        }

        // Получить оборудование по ID
        public async Task<EquipmentItem?> GetByIdAsync(int id)
        {
            SetAuthHeader();
            var response = await _httpClient.GetAsync($"api/equipment/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<EquipmentItem>();
        }

        // Создать новое оборудование (только Admin)
        public async Task<HttpResponseMessage> CreateAsync(EquipmentItem item)
        {
            SetAuthHeader();
            return await _httpClient.PostAsJsonAsync("api/equipment", item);
        }
            
        // Обновить оборудование (только Admin)
        public async Task<HttpResponseMessage> UpdateAsync(int id, EquipmentItem item)
        {
            SetAuthHeader();
            return await _httpClient.PutAsJsonAsync($"api/equipment/{id}", item);
        }

        // Удалить оборудование (только Admin)
        public async Task<HttpResponseMessage> DeleteAsync(int id)
        {
            SetAuthHeader();
            return await _httpClient.DeleteAsync($"api/equipment/{id}");
        }
    }
}
