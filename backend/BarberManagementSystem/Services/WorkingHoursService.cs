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

    // VALIDATION HELPER
    private static void ValidateWorkingHours(string dayOfWeek, TimeSpan start, TimeSpan end)
    {
        // Validate day name
        if (!Enum.TryParse<DayOfWeek>(dayOfWeek, out _))
            throw new Exception("Invalid DayOfWeek value. Must match .NET DayOfWeek names (e.g., Monday).");

        // Validate time range
        if (start >= end)
            throw new Exception("StartTime must be earlier than EndTime.");
    }

    // GET ALL (only active)
    public async Task<List<WorkingHoursResponseDto>> GetAllAsync()
    {
        return await _context.WorkingHours
            .Where(w => w.IsActive) // FIXED
            .Select(w => new WorkingHoursResponseDto
            {
                Id = w.Id,
                BarberId = w.BarberId,
                DayOfWeek = w.DayOfWeek,
                StartTime = w.StartTime,
                EndTime = w.EndTime,
                IsActive = w.IsActive
            })
            .ToListAsync();
    }

    // GET BY BARBER (only active)
    public async Task<List<WorkingHoursResponseDto>> GetByBarberAsync(int barberId)
    {
        return await _context.WorkingHours
            .Where(w => w.BarberId == barberId && w.IsActive) // FIXED
            .Select(w => new WorkingHoursResponseDto
            {
                Id = w.Id,
                BarberId = w.BarberId,
                DayOfWeek = w.DayOfWeek,
                StartTime = w.StartTime,
                EndTime = w.EndTime,
                IsActive = w.IsActive
            })
            .ToListAsync();
    }

    // CREATE
    public async Task<WorkingHoursResponseDto> CreateAsync(CreateWorkingHoursDto dto)
    {
        ValidateWorkingHours(dto.DayOfWeek, dto.StartTime, dto.EndTime);

        // Prevent duplicate day entries
        bool exists = await _context.WorkingHours
            .AnyAsync(w => w.BarberId == dto.BarberId
                        && w.DayOfWeek == dto.DayOfWeek
                        && w.IsActive);

        if (exists)
            throw new Exception("Working hours for this day already exist for this barber.");

        var entity = new WorkingHours
        {
            BarberId = dto.BarberId,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsActive = true
        };

        _context.WorkingHours.Add(entity);
        await _context.SaveChangesAsync();

        return new WorkingHoursResponseDto
        {
            Id = entity.Id,
            BarberId = entity.BarberId,
            DayOfWeek = entity.DayOfWeek,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            IsActive = entity.IsActive
        };
    }

    // UPDATE
    public async Task<WorkingHoursResponseDto> UpdateAsync(int id, UpdateWorkingHoursDto dto)
    {
        ValidateWorkingHours(dto.DayOfWeek, dto.StartTime, dto.EndTime);

        var entity = await _context.WorkingHours.FindAsync(id)
            ?? throw new Exception("Working hours not found.");

        // Prevent duplicates when updating
        bool duplicate = await _context.WorkingHours
            .AnyAsync(w => w.BarberId == entity.BarberId
                        && w.DayOfWeek == dto.DayOfWeek
                        && w.Id != id
                        && w.IsActive);

        if (duplicate)
            throw new Exception("Another working hours entry already exists for this day.");

        entity.DayOfWeek = dto.DayOfWeek;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return new WorkingHoursResponseDto
        {
            Id = entity.Id,
            BarberId = entity.BarberId,
            DayOfWeek = entity.DayOfWeek,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            IsActive = entity.IsActive
        };
    }

    // DELETE (soft delete)
    public async Task DeleteAsync(int id)
    {
        var entity = await _context.WorkingHours.FindAsync(id)
            ?? throw new Exception("Working hours not found.");

        entity.IsActive = false;

        await _context.SaveChangesAsync();
    }
}
