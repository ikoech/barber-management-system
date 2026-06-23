namespace BarberManagementSystem.DTOs.Booking;

public class AdminBookingOverviewDto
{
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    public int BarberId { get; set; }
    public string BarberName { get; set;} = string.Empty;

    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;

    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public string CustomerEmail { get; set; } = string.Empty;
    public string Status { get; set; } = "Confirmed";

}
