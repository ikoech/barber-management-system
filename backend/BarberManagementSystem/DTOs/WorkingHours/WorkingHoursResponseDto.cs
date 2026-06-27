namespace BarberManagementSystem.DTOs.WorkingHours;

public class WorkingHoursResponseDto
{
    public int Id { get; set; }
    public int BarberId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public string StartTime { get; set; } = "";   // FIXED
    public string EndTime { get; set; } = "";     // FIXED
    public bool IsActive { get; set; }
}
