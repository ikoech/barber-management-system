using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Services;

public class BookingEngine
{
    private readonly AppDbContext _context;

    public BookingEngine(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Booking> CreateBookingAsync(int userId, int barberId, int serviceId, DateTime start)
    {
        start = DateTime.SpecifyKind(start, DateTimeKind.Utc);

        // ⭐ NEW: Block booking if barber has a day off
        var dateOnly = DateOnly.FromDateTime(start);

        bool isDayOff = await _context.DayOffs
            .AnyAsync(d =>
                d.BarberId == barberId &&
                d.Date == dateOnly &&
                d.IsActive);

        if (isDayOff)
            throw new Exception("The barber is not available on this day.");

        var service = await _context.Services.FindAsync(serviceId)
            ?? throw new Exception("Service not found.");

        var duration = TimeSpan.FromMinutes(service.DurationMinutes);
        var end = DateTime.SpecifyKind(start.Add(duration), DateTimeKind.Utc);

        var day = start.DayOfWeek.ToString();

        var workingHours = await _context.WorkingHours
            .FirstOrDefaultAsync(w =>
                w.BarberId == barberId &&
                w.DayOfWeek == day &&
                start.TimeOfDay >= w.StartTime &&
                end.TimeOfDay <= w.EndTime);

        if (workingHours == null)
            throw new Exception("Barber is not working at this time.");

        var breakConflict = await _context.Breaks
            .AnyAsync(b =>
                b.BarberId == barberId &&
                b.Start.Date == start.Date &&
                start < b.End &&
                end > b.Start);

        if (breakConflict)
            throw new Exception("This time overlaps with a break.");

        var bookingConflict = await _context.Bookings
            .AnyAsync(b =>
                b.BarberId == barberId &&
                start < b.End &&
                end > b.Start);

        if (bookingConflict)
            throw new Exception("This time is already booked.");

        var booking = new Booking
        {
            UserId = userId,
            BarberId = barberId,
            ServiceId = serviceId,
            Start = start,
            End = end
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return booking;
    }
}
