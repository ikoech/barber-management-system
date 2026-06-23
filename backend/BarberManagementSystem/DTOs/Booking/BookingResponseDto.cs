namespace BarberManagementSystem.DTOs.Booking;

public class BookingResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BarberId { get; set; }
    public int ServiceId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public string BarberName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
}
