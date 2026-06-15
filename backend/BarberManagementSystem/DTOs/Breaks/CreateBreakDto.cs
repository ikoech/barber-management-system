namespace BarberManagementSystem.DTOs.Breaks;

public class CreateBreakDto
{
    public int BarberId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
