using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProgramlamaProje.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Foreign Key for AppUser (Member)
        [Required]
        public string AppUserId { get; set; } = string.Empty;
        [ForeignKey("AppUserId")]
        public virtual AppUser? AppUser { get; set; }

        // Foreign Key for Trainer
        public int TrainerId { get; set; }
        public virtual Trainer? Trainer { get; set; }

        // Foreign Key for Service
        public int ServiceId { get; set; }
        public virtual Service? Service { get; set; }
    }
}
