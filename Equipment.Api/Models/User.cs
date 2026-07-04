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
        public int Role_id { get; set; }     // Роль: 1 = User, 2 = Manager, 3 = Admin
        public Role Role { get; set; } = null!;
    }
}
