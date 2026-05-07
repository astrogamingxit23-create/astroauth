using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthAPI.Models
{
    public class Application
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string AppSecret { get; set; } = Guid.NewGuid().ToString();
        
        public Guid AdminId { get; set; }
        
        [ForeignKey("AdminId")]
        public Admin? Admin { get; set; }
        
        public ICollection<License> Licenses { get; set; } = new List<License>();
        public ICollection<SoftwareUser> Users { get; set; } = new List<SoftwareUser>();
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
