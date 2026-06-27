using BarberManagementSystem.DTOs.WorkingHours;
using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Services;

public class WorkingHoursService
{
    private readonly AppDbContext _context;

    public WorkingHoursService(AppDbContext context)
    {
        _context = context;
    }

    private static void ValidateWorkingHours(string dayOfWeek, TimeSpan start, TimeSpan end)
    {
        if (!Enum.TryParse<DayOfWeek>(dayOfWeek, out _))
            throw new Exception("Invalid DayOfWeek value.");

        if (start >= end)
            throw new Exception("StartTime must be earlier than EndTime.");
    }

    // GET ALL ACTIVE
    public async Task<List<WorkingHoursResponseDto>> GetAllAsync()
    {
        return await _context.WorkingHours
            .Where(w => w.IsActive)
            .Select(w => new WorkingHoursResponseDto
            {
                Id = w.Id,
                BarberId = w.BarberId,
                DayOfWeek = w.DayOfWeek,
                StartTime = w.StartTime.ToString(@"hh\:mm"),
                EndTime = w.EndTime.ToString(@"hh\:mm"),
                IsActive = w.IsActive
            })
            .ToListAsync();
    }

    // GET BY BARBER
    public async Task<List<WorkingHoursResponseDto>> GetByBarberAsync(int barberId)
    {
        return await _context.WorkingHours
            .Where(w => w.BarberId == barberId && w.IsActive)
            .Select(w => new WorkingHoursResponseDto
            {
                Id = w.Id,
                BarberId = w.BarberId,
                DayOfWeek = w.DayOfWeek,
                StartTime = w.StartTime.ToString(@"hh\:mm"),
                EndTime = w.EndTime.ToString(@"hh\:mm"),
                IsActive = w.IsActive
            })
            .ToListAsync();
    }

    // CREATE OR UPDATE (idempotent)
    public async Task<WorkingHoursResponseDto> CreateAsync(CreateWorkingHoursDto dto)
    {
        var start = TimeSpan.Parse(dto.StartTime);
        var end = TimeSpan.Parse(dto.EndTime);

        ValidateWorkingHours(dto.DayOfWeek, start, end);

        // Check if this day already exists
        var existing = await _context.WorkingHours
            .FirstOrDefaultAsync(w => w.BarberId == dto.BarberId &&
                                      w.DayOfWeek == dto.DayOfWeek &&
                                      w.IsActive);

        // If exists → update instead of throwing 400
        if (existing != null)
        {
            existing.StartTime = start;
            existing.EndTime = end;

            await _context.SaveChangesAsync();

            return new WorkingHoursResponseDto
            {
                Id = existing.Id,
                BarberId = existing.BarberId,
                DayOfWeek = existing.DayOfWeek,
                StartTime = existing.StartTime.ToString(@"hh\:mm"),
                EndTime = existing.EndTime.ToString(@"hh\:mm"),
                IsActive = existing.IsActive
            };
        }

        // Otherwise create new
        var entity = new WorkingHours
        {
            BarberId = dto.BarberId,
            DayOfWeek = dto.DayOfWeek.Trim(),
            StartTime = start,
            EndTime = end,
            IsActive = true
        };

        _context.WorkingHours.Add(entity);
        await _context.SaveChangesAsync();

        return new WorkingHoursResponseDto
        {
            Id = entity.Id,
            BarberId = entity.BarberId,
            DayOfWeek = entity.DayOfWeek,
            StartTime = entity.StartTime.ToString(@"hh\:mm"),
            EndTime = entity.EndTime.ToString(@"hh\:mm"),
            IsActive = entity.IsActive
        };
    }

    // UPDATE
    public async Task<WorkingHoursResponseDto> UpdateAsync(int id, UpdateWorkingHoursDto dto)
    {
        var entity = await _context.WorkingHours.FindAsync(id)
            ?? throw new Exception("Working hours not found.");

        var start = TimeSpan.Parse(dto.StartTime);
        var end = TimeSpan.Parse(dto.EndTime);

        ValidateWorkingHours(dto.DayOfWeek, start, end);

        // Check for duplicate day
        var duplicate = await _context.WorkingHours
            .AnyAsync(w => w.BarberId == entity.BarberId &&
                           w.DayOfWeek == dto.DayOfWeek &&
                           w.Id != id &&
                           w.IsActive);

        if (duplicate)
            throw new Exception("Another working hours entry already exists for this day.");

        entity.DayOfWeek = dto.DayOfWeek.Trim();
        entity.StartTime = start;
        entity.EndTime = end;
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return new WorkingHoursResponseDto
        {
            Id = entity.Id,
            BarberId = entity.BarberId,
            DayOfWeek = entity.DayOfWeek,
            StartTime = entity.StartTime.ToString(@"hh\:mm"),
            EndTime = entity.EndTime.ToString(@"hh\:mm"),
            IsActive = entity.IsActive
        };
    }

    // SOFT DELETE
    public async Task DeleteAsync(int id)
    {
        var entity = await _context.WorkingHours.FindAsync(id)
            ?? throw new Exception("Working hours not found.");

        entity.IsActive = false;

        await _context.SaveChangesAsync();
    }
}
