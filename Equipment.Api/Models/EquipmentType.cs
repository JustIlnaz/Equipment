using System.ComponentModel.DataAnnotations;

namespace Equipment.Api.Models
{
    public class EquipmentType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
