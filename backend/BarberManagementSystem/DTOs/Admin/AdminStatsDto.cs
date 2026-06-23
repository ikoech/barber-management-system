namespace BarberManagementSystem.DTOs.Admin;

public class AdminStatsDto
{
    public int TotalBookings { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalBarbers { get; set; }
    public int TotalServices { get; set; }
    public int TodayBookings { get; set; }
    public int UpcomingBookings { get; set; }
}
