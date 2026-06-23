using BarberManagementSystem.DTOs.Admin;
using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

public class AdminStatsService
{
    private readonly AppDbContext _context;

    public AdminStatsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdminStatsDto> GetStatsAsync()
    {
        var today = DateTime.UtcNow.Date;
            var now = today.AddDays(1);
        return new AdminStatsDto
        {
            TotalBookings = await _context.Bookings.CountAsync(),
            TotalCustomers = await _context.Users.CountAsync(u => u.Role == "Customer"),
            TotalBarbers = await _context.Users.CountAsync(u => u.Role == "Barber"),
            TotalServices = await _context.Services.CountAsync(),
            TodayBookings = await _context.Bookings.CountAsync(b => b.Start.Date == today),
            UpcomingBookings = await _context.Bookings.CountAsync(b => b.Start > today)
        };
    }
}
