using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProje.Models
{
    public class Trainer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        [StringLength(500)]
        public string? AvailabilityDescription { get; set; } // e.g., "Weekdays 9-5"

        // Navigation Properties
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
