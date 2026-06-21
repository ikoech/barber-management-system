namespace BarberManagementSystem.DTOs.Services;

public class CreateServiceDto
{
    public string Name { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public int BarberId { get; set; }

}
