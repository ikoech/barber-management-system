using BarberManagementSystem.DTOs.Booking;
using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

public class AdminBookingService
{
    private readonly AppDbContext _context;

    public AdminBookingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminBookingOverviewDto>> GetOverviewAsync(
        int? barberId, int? userId, int? serviceId, DateTime? date)
    {
        var query = _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Barber)
                .ThenInclude(br => br.User)
            .Include(b => b.Service)
            .AsQueryable();

        if (barberId.HasValue)
            query = query.Where(b => b.BarberId == barberId.Value);

        if (userId.HasValue)
            query = query.Where(b => b.UserId == userId.Value);

        if (serviceId.HasValue)
            query = query.Where(b => b.ServiceId == serviceId.Value);

        if (date.HasValue)
            query = query.Where(b => b.Start.Date == date.Value.Date);

        return await query
            .OrderBy(b => b.Start)
            .Select(b => new AdminBookingOverviewDto
            {
                BookingId = b.Id,

                UserId = b.UserId,
                UserName = b.User.FullName,
                CustomerEmail = b.User.Email,

                BarberId = b.BarberId,
                BarberName = b.Barber.User.FullName,

                ServiceId = b.ServiceId,
                ServiceName = b.Service.Name,

                Start = b.Start,
                End = b.End,

                Status = "Confirmed"
            })
            .ToListAsync();
    }
}
