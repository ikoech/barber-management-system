using BarberManagementSystem.DTOs.Barber;
using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Services;

public class ScheduleService
{
    private readonly AppDbContext _context;

    public ScheduleService(AppDbContext context)
    {
        _context = context;
    }

    // Get the schedule for a specific barber on a specific date
    public async Task<BarberScheduleDto> GetScheduleAsync(int barberId, DateTime date)
    {
        // Ensure barber exists
        var barberExists = await _context.Barbers.AnyAsync(b => b.Id == barberId);
        if (!barberExists)
        {
            return new BarberScheduleDto
            {
                Date = date,
                DayOfWeek = date.DayOfWeek.ToString(),
                AvailableSlots = new List<string>(),
                Breaks = new List<BreakDto>(),
                Bookings = new List<BookingSlotDto>()
            };
        }

        date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        var day = date.DayOfWeek.ToString();

        var schedule = new BarberScheduleDto
        {
            Date = date,
            DayOfWeek = day,
            Breaks = new List<BreakDto>(),
            Bookings = new List<BookingSlotDto>(),
            AvailableSlots = new List<string>()
        };

        // Working hours
        var working = await _context.WorkingHours
            .FirstOrDefaultAsync(w => w.BarberId == barberId && w.DayOfWeek == day);

        if (working != null)
        {
            schedule.WorkingStart = working.StartTime;
            schedule.WorkingEnd = working.EndTime;
        }

        // Breaks
        var breaks = await _context.Breaks
            .Where(b => b.BarberId == barberId && b.DayOfWeek == day)
            .ToListAsync();

        foreach (var br in breaks)
        {
            schedule.Breaks.Add(new BreakDto
            {
                Start = br.StartTime,
                End = br.EndTime
            });
        }

        // Bookings
        var bookings = await _context.Bookings
            .Where(b => b.BarberId == barberId && b.Start.Date == date.Date)
            .ToListAsync();

        foreach (var b in bookings)
        {
            schedule.Bookings.Add(new BookingSlotDto
            {
                Start = b.Start,
                End = b.End
            });
        }

        // Available slots (15 min increments)
        if (working != null)
        {
            var slots = new List<string>();
            var cursor = date.Date + working.StartTime;
            var end = date.Date + working.EndTime;

            while (cursor < end)
            {
                bool overlapsBooking = bookings.Any(b =>
                    cursor < b.End && cursor.AddMinutes(15) > b.Start);

                bool overlapsBreak = breaks.Any(br =>
                    cursor.TimeOfDay < br.EndTime &&
                    cursor.AddMinutes(15).TimeOfDay > br.StartTime);

                if (!overlapsBooking && !overlapsBreak)
                    slots.Add(cursor.ToString("HH:mm"));

                cursor = cursor.AddMinutes(15);
            }

            schedule.AvailableSlots = slots;
        }

        return schedule;
    }

    // Weekly schedule
    public async Task<WeeklyBarberScheduleDto> GetWeeklyScheduleAsync(int barberId, DateTime startDate)
    {
        startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);

        while (startDate.DayOfWeek != DayOfWeek.Monday)
            startDate = startDate.AddDays(-1);

        var week = new WeeklyBarberScheduleDto
        {
            WeekStart = startDate,
            WeekEnd = startDate.AddDays(6),
            Days = new List<BarberScheduleDto>()
        };

        for (int i = 0; i < 7; i++)
        {
            var day = startDate.AddDays(i);
            var daily = await GetScheduleAsync(barberId, day);
            week.Days.Add(daily);
        }

        return week;
    }

    // Monthly schedule
    public async Task<List<BarberScheduleDto>> GetMonthlyScheduleAsync(int barberId, int year, int month)
    {
        var schedules = new List<BarberScheduleDto>();
        var daysInMonth = DateTime.DaysInMonth(year, month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(year, month, day);
            var dailySchedule = await GetScheduleAsync(barberId, date);
            schedules.Add(dailySchedule);
        }

        return schedules;
    }
}
