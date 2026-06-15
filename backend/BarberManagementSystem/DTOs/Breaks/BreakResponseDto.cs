namespace BarberManagementSystem.DTOs.Breaks;

public class BreakResponseDto
{
    public int Id { get; set; }
    public int BarberId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool IsActive { get; set; }
}
