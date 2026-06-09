namespace BarberManagementSystem.DTOs.WorkingHours;

public class UpdateWorkingHoursDto
{
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
