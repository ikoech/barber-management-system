using System.ComponentModel.DataAnnotations;

namespace BarberManagementSystem.Models;

public class Service
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int DurationMinutes { get; set; }

    [Required]
    public decimal Price { get; set; }

    // Navigation
    public ICollection<Booking>? Bookings { get; set; }
}