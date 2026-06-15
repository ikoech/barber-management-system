namespace BarberManagementSystem.DTOs.WorkingHours;

public class WorkingHoursResponseDto
{
    public int Id { get; set; }
    public int BarberId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; }
}
