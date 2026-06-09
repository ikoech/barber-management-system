using BarberManagementSystem.DTOs.Barber;
using BarberManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class BarbersController : ControllerBase
{
    private readonly AppDbContext _context;
    public BarbersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/barbers
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

    // GET: api/barbers/{id}
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

    // POST: api/barbers
    [HttpPost]
    public async Task<IActionResult> Create(CreateBarberDto dto)
    {
        // Validate that the user exists and is not already a barber
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

    // PUT: api/barbers/{id}
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

    // DELETE: api/barbers/{id}
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
