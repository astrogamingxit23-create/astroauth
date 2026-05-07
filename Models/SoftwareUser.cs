using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthAPI.Models
{
    public class SoftwareUser
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public string? HWID { get; set; }
        
        public string? IP { get; set; }
        
        public DateTime? LastLogin { get; set; }
        
        public DateTime ExpiryDate { get; set; }
        
        public Guid ApplicationId { get; set; }
        
        [ForeignKey("ApplicationId")]
        public Application? Application { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
