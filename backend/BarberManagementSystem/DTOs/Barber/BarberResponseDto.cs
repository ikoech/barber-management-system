namespace BarberManagementSystem.DTOs.Barber;

public class BarberResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }

    // Must always be present for frontend safety
    public string FullName { get; set; } = string.Empty;

    public string Specialization { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
