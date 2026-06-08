using System.ComponentModel.DataAnnotations;
namespace BarberManagementSystem.Models;

public class WorkingHours
{

    public int Id { get; set; }

    [Required]
    public int BarberId { get; set; }

    [Required]
    public DayOfWeek Day { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    // Navigation
    public Barber? Barber { get; set; }
}
