using System.ComponentModel.DataAnnotations;

namespace Equipment.Api.Models
{
    // Модель пользователя — соответствует таблице Users в БД
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Login { get; set; } = string.Empty;    // Логин
        public string Password { get; set; } = string.Empty; // Пароль (хранится в открытом виде)
        public string Role { get; set; } = string.Empty;     // Роль: Admin или User
    }
}
