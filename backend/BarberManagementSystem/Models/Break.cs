using System.ComponentModel.DataAnnotations;

namespace BarberManagementSystem.Models;

public class Break
{
    public int Id { get; set; }

    [Required]
    public int BarberId { get; set; }

    [Required]
    [MaxLength(20)]
    public string DayOfWeek { get; set; } = string.Empty;

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    public Barber Barber { get; set; } = null!;
}
