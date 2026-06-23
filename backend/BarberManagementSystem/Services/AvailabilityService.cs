using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Services;

public class AvailabilityService
{
    private readonly AppDbContext _context;

    public AvailabilityService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DateTime>> GetAvailabilityAsync(int barberId, int serviceId, DateTime date)
    {
        // Convert incoming date to DateOnly (no timezone issues)
        var dateOnly = DateOnly.FromDateTime(date);
        var day = dateOnly.DayOfWeek.ToString();

        var service = await _context.Services.FindAsync(serviceId)
            ?? throw new Exception("Service not found.");

        var duration = TimeSpan.FromMinutes(service.DurationMinutes);

        // ⭐ Check Days Off
        bool isDayOff = await _context.DayOffs
            .AnyAsync(d =>
                d.BarberId == barberId &&
                d.Date == dateOnly &&
                d.IsActive);

        if (isDayOff)
            return new List<DateTime>();

        // ⭐ Get working hours
        var workingHours = await _context.WorkingHours
            .FirstOrDefaultAsync(w =>
                w.BarberId == barberId &&
                w.DayOfWeek == day &&
                w.IsActive);

        if (workingHours == null)
            return new List<DateTime>();

        // ⭐ Convert TimeSpan → DateTime
        var start = dateOnly.ToDateTime(TimeOnly.FromTimeSpan(workingHours.StartTime));
        var end = dateOnly.ToDateTime(TimeOnly.FromTimeSpan(workingHours.EndTime));

        // ⭐ Get breaks
        var breaks = await _context.Breaks
            .Where(b =>
                b.BarberId == barberId &&
                DateOnly.FromDateTime(b.Start) == dateOnly &&
                b.IsActive)
            .ToListAsync();

        // ⭐ Get bookings
        var bookings = await _context.Bookings
            .Where(b =>
                b.BarberId == barberId &&
                DateOnly.FromDateTime(b.Start) == dateOnly)
            .ToListAsync();

        // ⭐ Generate 15-minute slots
        var slots = new List<DateTime>();
        var current = start;

        while (current + duration <= end)
        {
            slots.Add(current);
            current = current.AddMinutes(15);
        }

        // ⭐ Filter breaks
        slots = slots
            .Where(slot =>
                !breaks.Any(br =>
                    slot < br.End &&
                    (slot + duration) > br.Start))
            .ToList();

        // ⭐ Filter bookings
        slots = slots
            .Where(slot =>
                !bookings.Any(b =>
                    slot < b.End &&
                    (slot + duration) > b.Start))
            .ToList();

        return slots;
    }
}
