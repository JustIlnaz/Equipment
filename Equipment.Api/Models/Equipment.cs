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
        
        public int? EquipmentTypeId { get; set; }
        public EquipmentType? EquipmentType { get; set; }

        public string SerialNumber { get; set; } = string.Empty;     // Серийный номер
        
        public int EquipmentStatusId { get; set; }
        public EquipmentStatus EquipmentStatus { get; set; } = null!;

        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
