using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BarberManagementSystem.Models;
using BarberManagementSystem.DTOs.Breaks;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class BreaksController : ControllerBase
{
    private readonly AppDbContext _context;

    public BreaksController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/breaks/barber/3
    [HttpGet("barber/{barberId}")]
    public async Task<IActionResult> GetByBarber(int barberId)
    {
        var breaks = await _context.Breaks
            .Where(b => b.BarberId == barberId)
            .Select(b => new BreakResponseDto
            {
                Id = b.Id,
                BarberId = b.BarberId,
                DayOfWeek = b.DayOfWeek,
                StartTime = b.StartTime,
                EndTime = b.EndTime
            })
            .ToListAsync();

        return Ok(breaks);
    }

    // POST: api/breaks
    [HttpPost]
    public async Task<IActionResult> Create(CreateBreakDto dto)
    {
        var barber = await _context.Barbers.FindAsync(dto.BarberId);
        if (barber == null)
            return BadRequest("Barber does not exist.");

        var brk = new Break
        {
            BarberId = dto.BarberId,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime
        };

        _context.Breaks.Add(brk);
        await _context.SaveChangesAsync();

        return Ok(new BreakResponseDto
        {
            Id = brk.Id,
            BarberId = brk.BarberId,
            DayOfWeek = brk.DayOfWeek,
            StartTime = brk.StartTime,
            EndTime = brk.EndTime
        });
    }

    // PUT: api/breaks/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateBreakDto dto)
    {
        var brk = await _context.Breaks.FindAsync(id);
        if (brk == null)
            return NotFound("Break not found.");

        brk.DayOfWeek = dto.DayOfWeek;
        brk.StartTime = dto.StartTime;
        brk.EndTime = dto.EndTime;

        await _context.SaveChangesAsync();

        return Ok(new BreakResponseDto
        {
            Id = brk.Id,
            BarberId = brk.BarberId,
            DayOfWeek = brk.DayOfWeek,
            StartTime = brk.StartTime,
            EndTime = brk.EndTime
        });
    }

    // DELETE: api/breaks/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var brk = await _context.Breaks.FindAsync(id);
        if (brk == null)
            return NotFound("Break not found.");

        _context.Breaks.Remove(brk);
        await _context.SaveChangesAsync();

        return Ok("Break deleted.");
    }
}
