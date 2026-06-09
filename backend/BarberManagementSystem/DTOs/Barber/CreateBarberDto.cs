namespace BarberManagementSystem.DTOs.Barber;

public class CreateBarberDto
{
    public int UserId { get; set; }
    public string Specialization { get; set; } = string.Empty;
}
