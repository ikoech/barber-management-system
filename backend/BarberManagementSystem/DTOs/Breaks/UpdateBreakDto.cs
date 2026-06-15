namespace BarberManagementSystem.DTOs.Breaks;

public class UpdateBreakDto
{
    public string DayOfWeek { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool IsActive { get; set; }
}
