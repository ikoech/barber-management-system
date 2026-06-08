
using System.ComponentModel.DataAnnotations;
namespace BarberManagementSystem.Models;

public class Break
{
    public int Id { get; set; }

    [Required]
    public int BarberId { get; set; }

    [Required]
    public DateTime Start { get; set; }

    [Required]
    public DateTime End { get; set; }

    // Navigation
    public Barber? Barber { get; set; }
}
