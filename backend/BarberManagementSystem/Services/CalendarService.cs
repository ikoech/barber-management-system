using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Services;

public class CalendarService
{
    private readonly AppDbContext _context;

    public CalendarService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CalendarResponse> GetCalendarAsync(int barberId, DateOnly month)
    {
        // 1. Load all bookings for the month
        var bookings = await _context.Bookings
            .Where(b =>
                b.BarberId == barberId &&
                DateOnly.FromDateTime(b.Start).Month == month.Month &&
                DateOnly.FromDateTime(b.Start).Year == month.Year)
            .ToListAsync();

        // 2. Load all days off for the month
        var daysOff = await _context.DayOffs
            .Where(d =>
                d.BarberId == barberId &&
                d.Date.Month == month.Month &&
                d.Date.Year == month.Year &&
                d.IsActive)
            .ToListAsync();

        // 3. Build calendar structure
        var days = new List<CalendarDay>();

        int daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateOnly(month.Year, month.Month, day);

            bool isDayOff = daysOff.Any(d => d.Date == date);

            var dayBookings = bookings
                .Where(b => DateOnly.FromDateTime(b.Start) == date)
                .ToList();

            days.Add(new CalendarDay
            {
                Date = date,
                IsDayOff = isDayOff,
                Bookings = isDayOff ? new List<Booking>() : dayBookings
            });
        }

        return new CalendarResponse
        {
            BarberId = barberId,
            Month = month,
            Days = days
        };
    }
}

public class CalendarResponse
{
    public int BarberId { get; set; }
    public DateOnly Month { get; set; }
    public List<CalendarDay> Days { get; set; } = new();
}

public class CalendarDay
{
    public DateOnly Date { get; set; }
    public bool IsDayOff { get; set; }
    public List<Booking> Bookings { get; set; } = new();
}
