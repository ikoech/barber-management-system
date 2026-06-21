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
        // FIX 1: Ensure incoming date is UTC
        date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

        var service = await _context.Services.FindAsync(serviceId)
            ?? throw new Exception("Service not found.");

        var duration = TimeSpan.FromMinutes(service.DurationMinutes);
        var day = date.DayOfWeek.ToString();

        // 1. Get working hours
        var workingHours = await _context.WorkingHours
            .FirstOrDefaultAsync(w =>
                w.BarberId == barberId &&
                w.DayOfWeek == day);

        if (workingHours == null)
            return new List<DateTime>();

        // 🔥 FIX 2: Ensure day is UTC before adding times
        var dayUtc = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);

        var start = DateTime.SpecifyKind(dayUtc + workingHours.StartTime, DateTimeKind.Utc);
        var end = DateTime.SpecifyKind(dayUtc + workingHours.EndTime, DateTimeKind.Utc);

        // 2. Get breaks
        var breaks = await _context.Breaks
            .Where(b =>
                b.BarberId == barberId &&
                b.Start.Date == date.Date &&
                b.IsActive)
            .ToListAsync();

        // 3. Get existing bookings
        var bookings = await _context.Bookings
            .Where(b =>
                b.BarberId == barberId &&
                b.Start.Date == date.Date)
            .ToListAsync();

        // 4. Generate 15-minute slots
        var slots = new List<DateTime>();
        var current = start;

        while (current + duration <= end)
        {
            // FIX 3: Ensure each generated slot is UTC
            slots.Add(DateTime.SpecifyKind(current, DateTimeKind.Utc));
            current = current.AddMinutes(15);
        }

        // 5. Filter out breaks
        slots = slots
            .Where(slot =>
                !breaks.Any(br =>
                    slot < br.End &&
                    (slot + duration) > br.Start))
            .ToList();

        // 6. Filter out bookings
        slots = slots
            .Where(slot =>
                !bookings.Any(b =>
                    slot < b.End &&
                    (slot + duration) > b.Start))
            .ToList();

        return slots;
    }
}
