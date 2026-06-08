using System.ComponentModel.DataAnnotations;

namespace BarberManagementSystem.Models;

public class Barber
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Specialization { get; set; } = string.Empty;

    // Navigation
    public User User { get; set; }
    public ICollection<WorkingHours>? WorkingHours { get; set; }
    public ICollection<Break>? Breaks { get; set; }
    public ICollection<Booking>? Bookings { get; set; }
}