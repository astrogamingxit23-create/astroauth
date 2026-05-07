using System.ComponentModel.DataAnnotations;

namespace AuthAPI.Models
{
    public class Admin
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string Plan { get; set; } = "Free"; // Free, Premium, Enterprise
        
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
