using System.ComponentModel.DataAnnotations;

namespace Equipment.Api.Models
{
    // Модель оборудования — соответствует таблице Equipment в БД
    public class EquipmentItem
    {
        [Key]
        public int Id { get; set; }

        public string InventoryNumber { get; set; } = string.Empty; // Инвентарный номер
        public string Name { get; set; } = string.Empty;             // Название
        public string Type { get; set; } = string.Empty;             // Тип оборудования
        public string SerialNumber { get; set; } = string.Empty;     // Серийный номер
        public string Status { get; set; } = string.Empty;           // Статус (Работает, В ремонте и т.д.)
        public string ResponsiblePerson { get; set; } = string.Empty; // Ответственный сотрудник
    }
}
