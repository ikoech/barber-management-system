using BarberManagementSystem.DTOs.Booking;
using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.DTOs.Services;

public class AdminBookingService
{
    private readonly AppDbContext _context;
    public AdminBookingService(AppDbContext context)
    {
        _context = context;
    }

    // Method to get all bookings with user, barber, and service details
    public async Task<List<AdminBookingOverviewDto>> GetOverviewAsync(
        
        int? barberId,
        int? userId,
        int? serviceId,
        DateTime? date)
    {
      var query = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Barber)
            .Include(b => b.Service)
            .AsQueryable();
        
        if (barberId.HasValue)
            query = query.Where(b => b.BarberId == barberId.Value);
        
        if (userId.HasValue)
            query = query.Where(b => b.UserId == userId.Value);
        
        if (serviceId.HasValue)
            query = query.Where(b => b.ServiceId == serviceId.Value);

        // Filter by date (if provided, we check if the booking's start date matches)
        if (date.HasValue)
        {
            var dayStart = DateTime.SpecifyKind(date.Value.Date, DateTimeKind.Utc);
            var dayEnd = dayStart.AddDays(1);

            query = query.Where(b => b.Start >= dayStart && b.Start < dayEnd);
        }

        var bookings = await query
            .OrderBy(b => b.Start)
            .Select(b => new AdminBookingOverviewDto
        {
            BookingId = b.Id,
            UserId = b.UserId,
            UserName = b.User.FullName,
            BarberId = b.BarberId,
            BarberName = b.Barber.User.FullName,
            ServiceId = b.ServiceId,
            ServiceName = b.Service.Name,
            Start = b.Start,
            End = b.End
        })
        .ToListAsync();

        return bookings;
    }
}
