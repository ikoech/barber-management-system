namespace BarberManagementSystem.DTOs.Barber;

public class BarberResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
