using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProgramlamaProje.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 600)]
        public int DurationMinutes { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        // Foreign Key for Gym
        public int GymId { get; set; }
        public virtual Gym? Gym { get; set; }

        // Navigation Properties
        public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
