using BarberManagementSystem.DTOs.DayOff;
using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Services;

public class DaysOffService
{
    private readonly AppDbContext _context;

    public DaysOffService(AppDbContext context)
    {
        _context = context;
    }

    // CREATE DAY OFF
    public async Task<DayOffResponseDto> CreateDayOffAsync(CreateDayOffDto dto)
    {
        // Validate barber exists
        var barber = await _context.Barbers
            .FirstOrDefaultAsync(b => b.Id == dto.BarberId)
            ?? throw new Exception("Barber not found.");

        // Validate duplicate day off
        bool exists = await _context.DayOffs
            .AnyAsync(d => d.BarberId == dto.BarberId && d.Date == dto.Date && d.IsActive);

        if (exists)
            throw new Exception("This day is already marked as a day off.");

        // Validate no bookings exist on this date
        bool hasBookings = await _context.Bookings
            .AnyAsync(b => b.BarberId == dto.BarberId && DateOnly.FromDateTime(b.Start.Date) == dto.Date);

        if (hasBookings)
            throw new Exception("Cannot mark this day off because bookings already exist.");

        var dayOff = new DayOff
        {
            BarberId = dto.BarberId,
            Date = dto.Date,
            Reason = dto.Reason,
            IsActive = true
        };

        _context.DayOffs.Add(dayOff);
        await _context.SaveChangesAsync();

        return new DayOffResponseDto
        {
            Id = dayOff.Id,
            BarberId = dayOff.BarberId,
            Date = dayOff.Date,
            Reason = dayOff.Reason,
            IsActive = dayOff.IsActive
        };
    }

    // GET ALL DAYS OFF FOR A BARBER
    public async Task<List<DayOffResponseDto>> GetDaysOffForBarberAsync(int barberId)
    {
        return await _context.DayOffs
            .Where(d => d.BarberId == barberId && d.IsActive)
            .OrderBy(d => d.Date)
            .Select(d => new DayOffResponseDto
            {
                Id = d.Id,
                BarberId = d.BarberId,
                Date = d.Date,
                Reason = d.Reason,
                IsActive = d.IsActive
            })
            .ToListAsync();
    }

    // DELETE DAY OFF
    public async Task DeleteDayOffAsync(int id, int barberId)
    {
        var dayOff = await _context.DayOffs
            .FirstOrDefaultAsync(d => d.Id == id && d.BarberId == barberId)
            ?? throw new Exception("Day off not found.");

        _context.DayOffs.Remove(dayOff);
        await _context.SaveChangesAsync();
    }
}
