using BarberManagementSystem.DTOs.Barber;
using BarberManagementSystem.Models;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Any logged-in user can reach this controller
public class BarbersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ScheduleService _scheduleService;

    public BarbersController(AppDbContext context, ScheduleService scheduleService)
    {
        _context = context;
        _scheduleService = scheduleService;
    }

    // ADMIN: GET ALL BARBERS
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var barbers = await _context.Barbers
            .Select(b => new BarberResponseDto
            {
                Id = b.Id,
                UserId = b.UserId,
                Specialization = b.Specialization
            })
            .ToListAsync();

        return Ok(barbers);
    }

    // ADMIN: GET BARBER BY ID
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var barber = await _context.Barbers
            .Where(b => b.Id == id)
            .Select(b => new BarberResponseDto
            {
                Id = b.Id,
                UserId = b.UserId,
                Specialization = b.Specialization
            })
            .FirstOrDefaultAsync();

        if (barber == null)
            return NotFound("Barber not found.");

        return Ok(barber);
    }

    // ADMIN: CREATE BARBER
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateBarberDto dto)
    {
        var user = await _context.Users.FindAsync(dto.UserId);
        if (user == null)
            return BadRequest("User not found.");

        var barber = new Barber
        {
            UserId = dto.UserId,
            Specialization = dto.Specialization
        };

        _context.Barbers.Add(barber);
        await _context.SaveChangesAsync();

        return Ok(new BarberResponseDto
        {
            Id = barber.Id,
            UserId = barber.UserId,
            Specialization = barber.Specialization
        });
    }

    // ADMIN: UPDATE BARBER
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateBarberDto dto)
    {
        var barber = await _context.Barbers.FindAsync(id);
        if (barber == null)
            return NotFound("Barber not found.");

        barber.Specialization = dto.Specialization;
        await _context.SaveChangesAsync();

        return Ok(new BarberResponseDto
        {
            Id = barber.Id,
            UserId = barber.UserId,
            Specialization = barber.Specialization
        });
    }

    // BARBER + ADMIN: DAILY SCHEDULE
    [Authorize(Policy = "BarberOrAdmin")]
    [HttpGet("{barberId}/schedule")]
    public async Task<IActionResult> GetSchedule(int barberId, [FromQuery] DateTime date)
    {
        var result = await _scheduleService.GetScheduleAsync(barberId, date);
        return Ok(result);
    }

    // BARBER + ADMIN: WEEKLY SCHEDULE
    [Authorize(Policy = "BarberOrAdmin")]
    [HttpGet("{barberId}/schedule/monthly")]
    public async Task<IActionResult> GetMonthlySchedule(int barberId, int year, int month)
    {
        var result = await _scheduleService.GetMonthlyScheduleAsync(barberId, year, month);

        if (result == null)
            return Ok(new { message = "No schedule found", barberId, year, month });
        Console.WriteLine($"Monthly schedule hit: barberId={barberId}, year={year}, month={month}");

        return Ok(result);
    }

    // ADMIN: DELETE BARBER
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var barber = await _context.Barbers.FindAsync(id);
        if (barber == null)
            return NotFound("Barber not found.");

        _context.Barbers.Remove(barber);
        await _context.SaveChangesAsync();

        return Ok("Barber deleted successfully.");
    }
}
