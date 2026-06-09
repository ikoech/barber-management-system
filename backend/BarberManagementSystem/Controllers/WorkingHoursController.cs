using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BarberManagementSystem.DTOs.WorkingHours;
using BarberManagementSystem.Models;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class WorkingHoursController : ControllerBase
{
    private readonly AppDbContext _context;
    public WorkingHoursController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/workinghours/barber/{barberId}
    [HttpGet("barber/{barberId}")]
    public async Task<IActionResult> GetByBarberId(int barberId)
    {
        var workingHours = await _context.WorkingHours
            .Where(wh => wh.BarberId == barberId)
            .Select(wh => new WorkingHoursResponseDto
            {
                Id = wh.Id,
                BarberId = wh.BarberId,
                DayOfWeek = wh.DayOfWeek,
                StartTime = wh.StartTime,
                EndTime = wh.EndTime
            })
            .ToListAsync();

        return Ok(workingHours);
    }

    // POST: api/workinghours
    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkingHoursDto dto)
    {
        var barber = await _context.Barbers.FindAsync(dto.BarberId);
        if (barber == null) 
            return BadRequest("Barber not found.");

        var workingHours = new WorkingHours
        {
            BarberId = dto.BarberId,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime
        };

        _context.WorkingHours.Add(workingHours);
        await _context.SaveChangesAsync();

        return Ok(new WorkingHoursResponseDto
        {
            Id = workingHours.Id,
            BarberId = workingHours.BarberId,
            DayOfWeek = workingHours.DayOfWeek,
            StartTime = workingHours.StartTime,
            EndTime = workingHours.EndTime
        });
    }

    // PUT: api/workinghours/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateWorkingHoursDto dto)
    {
        var workingHours = await _context.WorkingHours.FindAsync(id);
        if (workingHours == null)
            return NotFound("Working hours not found.");

        workingHours.DayOfWeek = dto.DayOfWeek;
        workingHours.StartTime = dto.StartTime;
        workingHours.EndTime = dto.EndTime;

        await _context.SaveChangesAsync();

        return Ok(new WorkingHoursResponseDto
        {
            Id = workingHours.Id,
            BarberId = workingHours.BarberId,
            DayOfWeek = workingHours.DayOfWeek,
            StartTime = workingHours.StartTime,
            EndTime = workingHours.EndTime
        });
    }

    // DELETE: api/workinghours/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var workingHours = await _context.WorkingHours.FindAsync(id);
        if (workingHours == null)
            return NotFound("Working hours not found.");


        _context.WorkingHours.Remove(workingHours);
        await _context.SaveChangesAsync();

        return Ok("Working hours deleted successfully.");
    }
}