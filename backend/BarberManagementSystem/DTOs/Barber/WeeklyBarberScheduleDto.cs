namespace BarberManagementSystem.DTOs.Barber;

public class WeeklyBarberScheduleDto
{
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
    public List<BarberScheduleDto> Days { get; set; } = new();
}
