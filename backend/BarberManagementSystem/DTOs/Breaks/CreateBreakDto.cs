namespace BarberManagementSystem.DTOs.Breaks;

public class CreateBreakDto
{
    public int BarberId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
