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
        dto.Start = DateTime.SpecifyKind(dto.Start, DateTimeKind.Utc);
        dto.End = DateTime.SpecifyKind(dto.End, DateTimeKind.Utc);

        var brk = new Break
        {
            BarberId = dto.BarberId,
            Start = dto.Start,
            End = dto.End,
            IsActive = true
        };

        _context.Breaks.Add(brk);
        await _context.SaveChangesAsync();

        return new BreakResponseDto
        {
            Id = brk.Id,
            BarberId = brk.BarberId,
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
                Start = b.Start,
                End = b.End,
                IsActive = b.IsActive
            })
            .ToListAsync();
    }

    // UPDATE
    public async Task<BreakResponseDto> UpdateAsync(int id, UpdateBreakDto dto)
    {
        dto.Start = DateTime.SpecifyKind(dto.Start, DateTimeKind.Utc);
        dto.End = DateTime.SpecifyKind(dto.End, DateTimeKind.Utc);

        var brk = await _context.Breaks.FindAsync(id)
            ?? throw new Exception("Break not found.");

        brk.Start = dto.Start;
        brk.End = dto.End;

        await _context.SaveChangesAsync();

        return new BreakResponseDto
        {
            Id = brk.Id,
            BarberId = brk.BarberId,
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
