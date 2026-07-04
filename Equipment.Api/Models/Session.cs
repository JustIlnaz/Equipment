using System.ComponentModel.DataAnnotations;

namespace Equipment.Api.Models
{
    public class Session
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
