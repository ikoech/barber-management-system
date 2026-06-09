using System.ComponentModel.DataAnnotations;

namespace BarberManagementSystem.Models
{
    public class Barber
    {
        public int Id { get; set; }

        // FK → User
        [Required]
        public int UserId { get; set; }

        // Required specialization field
        [Required]
        [MaxLength(100)]
        public string Specialization { get; set; } = string.Empty;

        // Navigation properties
        public User User { get; set; } = null!;  // non-nullable navigation

        public ICollection<WorkingHours> WorkingHours { get; set; } = new List<WorkingHours>();
        public ICollection<Break> Breaks { get; set; } = new List<Break>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
