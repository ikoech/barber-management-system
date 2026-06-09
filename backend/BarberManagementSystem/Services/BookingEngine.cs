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
        // Ensure incoming DateTime is UTC (fixes PostgreSQL timestamp with time zone error)
        start = DateTime.SpecifyKind(start, DateTimeKind.Utc);

        var barber = await _context.Barbers.FindAsync(barberId)
            ?? throw new Exception("Barber not found.");

        var service = await _context.Services.FindAsync(serviceId)
            ?? throw new Exception("Service not found.");

        var duration = TimeSpan.FromMinutes(service.DurationMinutes);
        var end = DateTime.SpecifyKind(start.Add(duration), DateTimeKind.Utc);

        var day = start.DayOfWeek.ToString();

        // Validate working hours
        var workingHours = await _context.WorkingHours
            .FirstOrDefaultAsync(w =>
                w.BarberId == barberId &&
                w.DayOfWeek == day &&
                start.TimeOfDay >= w.StartTime &&
                end.TimeOfDay <= w.EndTime);

        if (workingHours == null)
            throw new Exception("Barber is not working at this time.");

        // Validate break conflicts
        var breakConflict = await _context.Breaks
            .AnyAsync(b =>
                b.BarberId == barberId &&
                b.DayOfWeek == day &&
                start.TimeOfDay < b.EndTime &&
                end.TimeOfDay > b.StartTime);

        if (breakConflict)
            throw new Exception("This time overlaps with a break.");

        // Validate overlapping bookings
        var bookingConflict = await _context.Bookings
            .AnyAsync(b =>
                b.BarberId == barberId &&
                start < b.End &&
                end > b.Start);

        if (bookingConflict)
            throw new Exception("This time is already booked.");

        // Create booking
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
