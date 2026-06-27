namespace BarberManagementSystem.DTOs.WorkingHours;

public class CreateWorkingHoursDto
{
    public int BarberId { get; set; }
    public string DayOfWeek { get; set; } = "";   // "Monday"
    public string StartTime { get; set; } = "";   // "09:00"
    public string EndTime { get; set; } = "";     // "17:00"
}
