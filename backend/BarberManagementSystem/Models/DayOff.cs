namespace BarberManagementSystem.Models;

public class DayOff
{
    public int Id { get; set; }

    public int BarberId { get; set; }
    public Barber Barber { get; set; }

    public DateOnly Date { get; set; }

    public string? Reason { get; set; }

    public bool IsActive { get; set; } = true;
}
