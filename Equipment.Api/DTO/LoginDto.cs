namespace Equipment.Api.DTO
{
    // DTO для запроса авторизации
    public class LoginDto
    {
        public string Login { get; set; } = string.Empty;    // Логин
        public string Password { get; set; } = string.Empty; // Пароль
    }
}
