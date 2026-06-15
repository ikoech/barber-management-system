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

    //CREATE
    public async Task<WorkingHoursResponseDto> CreateAsync(CreateWorkingHoursDto dto)
    {
        var barberExist = await _context.Barbers.AnyAsync(b => b.Id == dto.BarberId);
        if (!barberExist)
            throw new Exception("Barber not found");

        var workingHours = new WorkingHours
        {
            BarberId = dto.BarberId,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            IsActive = true
        };

        _context.WorkingHours.Add(workingHours);
        await _context.SaveChangesAsync();

        return new WorkingHoursResponseDto
        {
            Id = workingHours.Id,
            BarberId = workingHours.BarberId,
            DayOfWeek = workingHours.DayOfWeek,
            StartTime = workingHours.StartTime,
            EndTime = workingHours.EndTime,
            IsActive = workingHours.IsActive
        };
    }
    // GET ALL (ADMIN)
    public async Task<List<WorkingHoursResponseDto>> GetAllAsync()
    {
        return await _context.WorkingHours
            .Where(w => w.IsActive)
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

    // GET BY BABRBER
    public async Task<List<WorkingHoursResponseDto>> GetByBarberAsync(int barberId)
    {
        return await _context.WorkingHours
            .Where(w => w.BarberId == barberId && w.IsActive)
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
    // UPDATE
    public async Task<WorkingHoursResponseDto> UpdateAsync(int id, UpdateWorkingHoursDto dto)
    {
        var workingHours = await _context.WorkingHours.FindAsync(id)
            ?? throw new Exception("Working hours not found.");

        workingHours.DayOfWeek = dto.DayOfWeek;
        workingHours.StartTime = dto.StartTime;
        workingHours.EndTime = dto.EndTime;

        await _context.SaveChangesAsync();

        return new WorkingHoursResponseDto
        {
            Id = workingHours.Id,
            BarberId = workingHours.BarberId,
            DayOfWeek = workingHours.DayOfWeek,
            StartTime = workingHours.StartTime,
            EndTime = workingHours.EndTime,
            IsActive = workingHours.IsActive
        };
    }
    // SOFT DELETE
    public async Task<bool> DeleteAsync(int id)
    {
        var workingHours = await _context.WorkingHours.FindAsync(id)
            ?? throw new Exception("Working hours not found.");

        workingHours.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }
}