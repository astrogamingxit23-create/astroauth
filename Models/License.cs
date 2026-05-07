using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthAPI.Models
{
    public class License
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Key { get; set; } = string.Empty;
        
        public int DurationDays { get; set; }
        
        public bool IsUsed { get; set; } = false;
        
        public DateTime? UsedAt { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
        
        public Guid ApplicationId { get; set; }
        
        [ForeignKey("ApplicationId")]
        public Application? Application { get; set; }
        
        public string? Note { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
