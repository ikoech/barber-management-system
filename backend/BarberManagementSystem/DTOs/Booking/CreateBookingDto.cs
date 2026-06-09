namespace BarberManagementSystem.DTOs.Booking;

public class CreateBookingDto
{
    public int UserId { get; set; }
    public int BarberId { get; set; }
    public int ServiceId { get; set; }
    public DateTime Start { get; set; }

}
