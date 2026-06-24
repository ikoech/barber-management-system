using BarberManagementSystem.DTOs.WorkingHours;
using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Services;

public class WorkingHoursAvailabilityService
{
    private readonly AppDbContext _context;

    public WorkingHoursAvailabilityService(AppDbContext context)
    {
        _context = context;
    }

    private static string ToHHmm(TimeSpan t) => t.ToString(@"hh\:mm");

    private static string NormalizeDayOfWeek(string dayOfWeek)
        => (dayOfWeek ?? string.Empty).Trim();

    private static DateTime UtcMidnight(DateOnly d) => d.ToDateTime(TimeOnly.MinValue).ToUniversalTime();

    private static bool IsReasonableUtcInstant(DateTime dt)
    {
        if (dt == default) return false;
        if (dt.Kind == DateTimeKind.Utc || dt.Kind == DateTimeKind.Unspecified || dt.Kind == DateTimeKind.Local)
        {
            // PostgreSQL timestamp with time zone supports a large range; we still guard against corrupt data.
            var min = DateTime.UnixEpoch.AddYears(-200);
            var max = DateTime.UnixEpoch.AddYears(200);
            return dt >= min && dt <= max;
        }
        return false;
    }

    private static DateTime EnsureUtcOrNull(DateTime dt)
    {
        try
        {
            if (dt.Kind == DateTimeKind.Utc) return dt;
            if (dt.Kind == DateTimeKind.Unspecified) return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            return dt.ToUniversalTime();
        }
        catch
        {
            return default;
        }
    }

    private static bool TryNormalizeUtcInstant(DateTime dt, out DateTime utc)
    {
        utc = default;
        if (!IsReasonableUtcInstant(dt)) return false;

        var normalized = EnsureUtcOrNull(dt);
        if (normalized == default) return false;
        if (!IsReasonableUtcInstant(normalized)) return false;

        utc = normalized.Kind == DateTimeKind.Utc ? normalized : DateTime.SpecifyKind(normalized, DateTimeKind.Utc);
        return true;
    }

    private static AvailableTimesResponseDto EmptyForDate(List<string> daysOff, DateOnly date)
    {
        var d = date.ToString("yyyy-MM-dd");
        return new AvailableTimesResponseDto
        {
            isWorking = false,
            workingHours = new List<WorkingHourRangeDto>(),
            breaks = new List<TimeRangeDto>(),
            availableTimes = new List<string>(),
            daysOff = (daysOff ?? new List<string>()).Where(x => x == d).Distinct().ToList()
        };
    }

    public async Task<AvailableTimesResponseDto> GetAvailabilityAsync(int barberId, DateOnly date, int serviceId, int stepMinutes)
    {
        var daysOff = await _context.DayOffs
            .Where(d => d.BarberId == barberId && d.IsActive)
            .Select(d => d.Date.ToString("yyyy-MM-dd"))
            .ToListAsync();

        try
        {
            if (barberId <= 0 || serviceId <= 0 || stepMinutes <= 0)
                return EmptyForDate(daysOff, date);

            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == serviceId && s.IsActive);
            if (service == null)
                return EmptyForDate(daysOff, date);

            var duration = TimeSpan.FromMinutes(service.DurationMinutes);
            if (duration <= TimeSpan.Zero)
                return EmptyForDate(daysOff, date);

            // Day off short-circuit
            if (await _context.DayOffs.AnyAsync(d => d.BarberId == barberId && d.IsActive && d.Date == date))
                return EmptyForDate(daysOff, date);

            var dayName = NormalizeDayOfWeek(date.DayOfWeek.ToString());

            var whRows = await _context.WorkingHours
                .Where(w => w.BarberId == barberId && w.IsActive && w.DayOfWeek == dayName)
                .ToListAsync();

            if (whRows.Count == 0)
                return EmptyForDate(daysOff, date);

            // Working-hours as HH:mm windows
            var workingHours = new List<WorkingHourRangeDto>();
            foreach (var w in whRows)
            {
                if (w.EndTime <= w.StartTime) continue;
                workingHours.Add(new WorkingHourRangeDto { start = ToHHmm(w.StartTime), end = ToHHmm(w.EndTime) });
            }

            if (workingHours.Count == 0)
                return EmptyForDate(daysOff, date);

            var dayStartUtc = UtcMidnight(date);
            var dayEndUtc = dayStartUtc.AddDays(1);

            // Breaks must be matched by DayOfWeek, not by date_trunc.
            // Note: Break model stores Start/End instants; we normalize their time-of-day for UI and UTC overlap checks.
            var breaksByDay = await _context.Breaks
                .Where(b => b.BarberId == barberId && b.IsActive && b.DayOfWeek == dayName)
                .ToListAsync();

            var normalizedBreakRanges = new List<(DateTime Start, DateTime End)>();
            var breakDtos = new List<TimeRangeDto>();

            foreach (var b in breaksByDay)
            {
                if (TryNormalizeUtcInstant(b.Start, out var bStartUtc) && TryNormalizeUtcInstant(b.End, out var bEndUtc))
                {
                    if (bEndUtc <= bStartUtc) continue;

                    // Force the break instants into the target day by time-of-day (UTC).
                    // This keeps overlap logic correct even if the stored date part is corrupted.
                    var startTod = bStartUtc.TimeOfDay;
                    var endTod = bEndUtc.TimeOfDay;

                    var startUtc = dayStartUtc.Add(startTod);
                    var endUtc = dayStartUtc.Add(endTod);

                    // If a break crosses midnight, clamp into the day interval safely.
                    if (endUtc <= startUtc) continue;
                    if (endUtc <= dayStartUtc || startUtc >= dayEndUtc) continue;

                    var clampedStart = startUtc < dayStartUtc ? dayStartUtc : startUtc;
                    var clampedEnd = endUtc > dayEndUtc ? dayEndUtc : endUtc;
                    if (clampedEnd <= clampedStart) continue;

                    normalizedBreakRanges.Add((clampedStart, clampedEnd));
                    breakDtos.Add(new TimeRangeDto { start = ToHHmm(clampedStart.TimeOfDay), end = ToHHmm(clampedEnd.TimeOfDay) });
                }
            }

            // Existing bookings conflicts (exclude overlapping bookings)
            var bookings = await _context.Bookings
                .Where(b => b.BarberId == barberId && b.Start < dayEndUtc && b.End > dayStartUtc)
                .Select(b => new { Start = b.Start, End = b.End })
                .ToListAsync();

            var normalizedBookings = new List<(DateTime Start, DateTime End)>();
            foreach (var bk in bookings)
            {
                if (!TryNormalizeUtcInstant(bk.Start, out var sUtc)) continue;
                if (!TryNormalizeUtcInstant(bk.End, out var eUtc)) continue;
                if (eUtc <= sUtc) continue;
                if (eUtc <= dayStartUtc || sUtc >= dayEndUtc) continue;

                var cs = sUtc < dayStartUtc ? dayStartUtc : sUtc;
                var ce = eUtc > dayEndUtc ? dayEndUtc : eUtc;
                if (ce <= cs) continue;

                normalizedBookings.Add((cs, ce));
            }

            var candidateTimes = new SortedSet<string>();
            var step = TimeSpan.FromMinutes(stepMinutes);

            foreach (var w in whRows)
            {
                if (w.EndTime <= w.StartTime) continue;

                var windowStart = w.StartTime;
                var windowEnd = w.EndTime;

                var latestStart = windowEnd - duration;
                if (latestStart < windowStart) continue;

                for (var current = windowStart; current <= latestStart; current = current.Add(step))
                {
                    var startUtc = dayStartUtc.Add(current);
                    var endUtc = startUtc.Add(duration);

                    if (endUtc > dayEndUtc) break;

                    // Overlaps with breaks
                    var overlapsBreak = normalizedBreakRanges.Any(br => startUtc < br.End && endUtc > br.Start);
                    if (overlapsBreak) continue;

                    // Overlaps with other bookings
                    var overlapsBooking = normalizedBookings.Any(bk => startUtc < bk.End && endUtc > bk.Start);
                    if (overlapsBooking) continue;

                    candidateTimes.Add(ToHHmm(current));
                }
            }

            return new AvailableTimesResponseDto
            {
                isWorking = true,
                workingHours = workingHours,
                breaks = breakDtos,
                daysOff = (daysOff ?? new List<string>()).Where(x => x == date.ToString("yyyy-MM-dd")).Distinct().ToList(),
                availableTimes = candidateTimes.ToList()
            };
        }
        catch
        {
            return EmptyForDate(daysOff, date);
        }
    }
}



