using System.ComponentModel.DataAnnotations;

namespace Equipment.Api.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
