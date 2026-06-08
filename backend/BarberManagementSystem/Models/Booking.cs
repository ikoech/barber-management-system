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
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }


    // Navigation
    public User? User { get; set; }
    public Barber? Barber { get; set; }
    public Service? Service { get; set; }
}
