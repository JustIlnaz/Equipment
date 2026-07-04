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
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<EquipmentStatus> EquipmentStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.Role_id);
        }
    }
}
