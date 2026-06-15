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
    public DateTime Start { get; set; }

    [Required]
    public DateTime End { get; set; }

    public Barber Barber { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
