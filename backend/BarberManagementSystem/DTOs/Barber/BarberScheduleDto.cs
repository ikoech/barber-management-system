using System;
using System.Collections.Generic;

namespace BarberManagementSystem.DTOs.Barber;

public class BarberScheduleDto
{
    public DateTime Date { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;

    public TimeSpan? WorkingStart { get; set; }
    public TimeSpan? WorkingEnd { get; set; }

    public List<BreakDto> Breaks { get; set; } = new();
    public List<BookingSlotDto> Bookings { get; set; } = new();
    public List<string> AvailableSlots { get; set; } = new();
}
