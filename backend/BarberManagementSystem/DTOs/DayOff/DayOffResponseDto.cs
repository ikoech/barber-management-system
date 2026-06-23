namespace BarberManagementSystem.DTOs.DayOff;

public class DayOffResponseDto
{
    public int Id { get; set; }
    public int BarberId { get; set; }
    public DateOnly Date { get; set; }
    public string? Reason { get; set; }
    public bool IsActive { get; set; }
}
