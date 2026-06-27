namespace BarberManagementSystem.DTOs.WorkingHours;

public class UpdateWorkingHoursDto
{
    public string DayOfWeek { get; set; } = string.Empty;
    public string StartTime { get; set; } = "";   // FIXED
    public string EndTime { get; set; } = "";     // FIXED
    public bool IsActive { get; set; }
}
