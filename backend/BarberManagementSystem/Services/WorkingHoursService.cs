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

    // GET ALL
    public async Task<List<WorkingHoursResponseDto>> GetAllAsync()
    {
        return await _context.WorkingHours
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

    // GET BY BARBER
    public async Task<List<WorkingHoursResponseDto>> GetByBarberAsync(int barberId)
    {
        return await _context.WorkingHours
            .Where(w => w.BarberId == barberId)
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
        var entity = await _context.WorkingHours.FindAsync(id)
            ?? throw new Exception("Working hours not found.");

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
