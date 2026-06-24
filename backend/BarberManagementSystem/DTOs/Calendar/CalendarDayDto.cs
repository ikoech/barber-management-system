namespace BarberManagementSystem.DTOs.Calendar;

public class CalendarDayDto
{
    public DateTime Date { get; set; }
    public bool IsDayOff { get; set; }
    public List<BookingSummaryDto> Bookings { get; set; } = new();
}
