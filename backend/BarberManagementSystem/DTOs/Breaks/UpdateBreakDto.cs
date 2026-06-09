namespace BarberManagementSystem.DTOs.Breaks;

public class UpdateBreakDto
{
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
