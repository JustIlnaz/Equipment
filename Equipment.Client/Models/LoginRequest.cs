namespace Equipment.Client.Models
{
    // Модель запроса для авторизации
    public class LoginRequest
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
