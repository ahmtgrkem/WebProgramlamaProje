using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class Gym
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string OpeningHours { get; set; } = string.Empty; // e.g., "09:00 - 22:00"

        [Phone]
        [StringLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        // Navigation Property
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
