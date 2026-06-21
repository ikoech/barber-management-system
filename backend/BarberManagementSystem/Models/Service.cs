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

    //Soft delete flag
    public bool IsActive { get; set; }

    // REQUIRED: Link service → barber
    [Required]
    public int BarberId { get; set; }
    public Barber Barber { get; set; } = null!;

    // Navigation
    public ICollection<Booking>? Bookings { get; set; }
}