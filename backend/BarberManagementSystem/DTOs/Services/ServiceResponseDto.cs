namespace BarberManagementSystem.DTOs.Services;

public class ServiceResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }


    //Expose IsActive when admin needs to see it
    public bool IsActive { get; set; }

}
