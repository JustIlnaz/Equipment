namespace Equipment.Api.DTO
{
    public class EquipmentItemDto
    {
        public int Id { get; set; }
        public string InventoryNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ResponsiblePerson { get; set; } = string.Empty;
    }
}
