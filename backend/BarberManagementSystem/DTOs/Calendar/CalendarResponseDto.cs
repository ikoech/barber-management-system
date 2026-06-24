namespace BarberManagementSystem.DTOs.Calendar;

public class CalendarResponseDto
{
    public int BarberId { get; set; }
    public string Month { get; set; } = string.Empty;
    public List<CalendarDayDto> Days { get; set; } = new();
}
