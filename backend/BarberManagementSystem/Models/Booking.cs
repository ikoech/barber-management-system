using System.ComponentModel.DataAnnotations;

namespace BarberManagementSystem.Models;

public class Booking
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int BarberId { get; set; }

    [Required]
    public int ServiceId { get; set; }

    [Required]
    public DateTime Start { get; set; }

    [Required]
    public DateTime End { get; set; }

    public User? User { get; set; }
    public Barber? Barber { get; set; }
    public Service? Service { get; set; }
}
