namespace Equipment.Client.Models
{
    // Модель ответа при авторизации — содержит JWT токен
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Login { get; set; } = string.Empty;
    }
}
