using BarberManagementSystem.DTOs.Breaks;
using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Services;

public class BreakService
{
    private readonly AppDbContext _context;

    public BreakService(AppDbContext context)
    {
        _context = context;
    }

    // CREATE
    public async Task<BreakResponseDto> CreateAsync(CreateBreakDto dto)
    {
        if (dto == null)
            throw new Exception("Missing break payload.");

        if (dto.BarberId <= 0)
            throw new Exception("barberId is required and must be > 0.");

        if (string.IsNullOrWhiteSpace(dto.DayOfWeek))
            throw new Exception("DayOfWeek is required.");

        if (!Enum.TryParse<DayOfWeek>(dto.DayOfWeek.Trim(), true, out var parsedDay))
            throw new Exception("Invalid DayOfWeek. Use Monday..Sunday.");

        if (dto.Start == default || dto.End == default)
            throw new Exception("Start and end timestamps are required.");

        if (dto.End <= dto.Start)
            throw new Exception("End must be after Start.");

        // Persist UTC instants
        var startUtc = dto.Start.Kind == DateTimeKind.Utc ? dto.Start : DateTime.SpecifyKind(dto.Start, DateTimeKind.Utc);
        var endUtc = dto.End.Kind == DateTimeKind.Utc ? dto.End : DateTime.SpecifyKind(dto.End, DateTimeKind.Utc);

        if (endUtc <= startUtc)
            throw new Exception("End must be after Start.");

        var brk = new Break
        {
            BarberId = dto.BarberId,
            DayOfWeek = parsedDay.ToString(),
            Start = startUtc,
            End = endUtc,
            IsActive = true
        };

        _context.Breaks.Add(brk);
        await _context.SaveChangesAsync();

        return new BreakResponseDto
        {
            Id = brk.Id,
            BarberId = brk.BarberId,
            DayOfWeek = brk.DayOfWeek,
            Start = brk.Start,
            End = brk.End,
            IsActive = brk.IsActive
        };
    }


    // GET ALL
    public async Task<List<BreakResponseDto>> GetAllAsync()
    {
        return await _context.Breaks
            .Where(b => b.IsActive)
            .Select(b => new BreakResponseDto
            {
                Id = b.Id,
                BarberId = b.BarberId,
                DayOfWeek = b.DayOfWeek,
                Start = b.Start,
                End = b.End,
                IsActive = b.IsActive
            })
            .ToListAsync();
    }


    // GET BY BARBER
    public async Task<List<BreakResponseDto>> GetByBarberAsync(int barberId)
    {
        return await _context.Breaks
            .Where(b => b.BarberId == barberId && b.IsActive)
            .Select(b => new BreakResponseDto
            {
                Id = b.Id,
                BarberId = b.BarberId,
                DayOfWeek = b.DayOfWeek,
                Start = b.Start,
                End = b.End,
                IsActive = b.IsActive
            })
            .ToListAsync();
    }


    // UPDATE
    public async Task<BreakResponseDto> UpdateAsync(int id, UpdateBreakDto dto)
    {
        if (dto == null)
            throw new Exception("Missing break payload.");

        if (id <= 0)
            throw new Exception("break id is required and must be > 0.");

        if (string.IsNullOrWhiteSpace(dto.DayOfWeek))
            throw new Exception("DayOfWeek is required.");

        if (!Enum.TryParse<DayOfWeek>(dto.DayOfWeek.Trim(), true, out var parsedDay))
            throw new Exception("Invalid DayOfWeek. Use Monday..Sunday.");

        if (dto.Start == default || dto.End == default)
            throw new Exception("Start and end timestamps are required.");

        if (dto.End <= dto.Start)
            throw new Exception("End must be after Start.");

        var startUtc = dto.Start.Kind == DateTimeKind.Utc ? dto.Start : DateTime.SpecifyKind(dto.Start, DateTimeKind.Utc);
        var endUtc = dto.End.Kind == DateTimeKind.Utc ? dto.End : DateTime.SpecifyKind(dto.End, DateTimeKind.Utc);

        var brk = await _context.Breaks.FindAsync(id)
            ?? throw new Exception("Break not found.");

        brk.DayOfWeek = parsedDay.ToString();
        brk.Start = startUtc;
        brk.End = endUtc;

        await _context.SaveChangesAsync();

        return new BreakResponseDto
        {
            Id = brk.Id,
            BarberId = brk.BarberId,
            DayOfWeek = brk.DayOfWeek,
            Start = brk.Start,
            End = brk.End,
            IsActive = brk.IsActive
        };
    }


    // SOFT DELETE
    public async Task<bool> DeleteAsync(int id)
    {
        var brk = await _context.Breaks.FindAsync(id)
            ?? throw new Exception("Break not found.");

        brk.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }
}
