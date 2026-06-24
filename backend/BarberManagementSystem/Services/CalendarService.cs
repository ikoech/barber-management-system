using BarberManagementSystem.DTOs.Calendar;
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

    public async Task<CalendarResponseDto> GetCalendarAsync(int barberId, DateOnly month)
    {
        int year = month.Year;
        int monthNumber = month.Month;

        var bookings = await _context.Bookings
            .Where(b => b.BarberId == barberId &&
                        b.Start.Year == year &&
                        b.Start.Month == monthNumber)
            .ToListAsync();

        var daysOff = await _context.DayOffs
            .Where(d => d.BarberId == barberId &&
                        d.IsActive &&
                        d.Date.Year == year &&
                        d.Date.Month == monthNumber)
            .ToListAsync();

        var days = new List<CalendarDayDto>();
        int daysInMonth = DateTime.DaysInMonth(year, monthNumber);

        for (int day = 1; day <= daysInMonth; day++)
        {
            var dateOnly = new DateOnly(year, monthNumber, day);
            var dateTime = dateOnly.ToDateTime(TimeOnly.MinValue);

            bool isDayOff = daysOff.Any(d => d.Date == dateOnly);

            var dayBookings = bookings
                .Where(b => DateOnly.FromDateTime(b.Start) == dateOnly)
                .Select(b => new BookingSummaryDto
                {
                    Id = b.Id,
                    Start = b.Start,
                    End = b.End
                })
                .ToList();

            days.Add(new CalendarDayDto
            {
                Date = dateTime,
                IsDayOff = isDayOff,
                Bookings = isDayOff ? new() : dayBookings
            });
        }

        return new CalendarResponseDto
        {
            BarberId = barberId,
            Month = month.ToString("yyyy-MM"),
            Days = days
        };
    }
}
