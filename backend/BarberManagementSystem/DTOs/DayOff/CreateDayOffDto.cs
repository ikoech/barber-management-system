namespace BarberManagementSystem.DTOs.DayOff;

public class CreateDayOffDto
{
    public int BarberId { get; set; }
    public DateOnly Date { get; set; }
    public string? Reason { get; set; }
}
