using Equipment.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Equipment.Api.Data
{
    // Контекст базы данных — связывает модели с таблицами PostgreSQL
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<EquipmentItem> Equipment { get; set; } // Таблица оборудования
        public DbSet<User> Users { get; set; }               // Таблица пользователей

        // Данные заполняются пользователем в БД напрямую
    }
}
