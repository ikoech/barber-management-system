using BarberManagementSystem.DTOs.Booking;
using BarberManagementSystem.Models;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingEngine _engine;
    private readonly AppDbContext _context;

    public BookingsController(BookingEngine engine, AppDbContext context)
    {
        _engine = engine;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingDto dto)
    {
        try
        {
            var booking = await _engine.CreateBookingAsync(
                dto.UserId,
                dto.BarberId,
                dto.ServiceId,
                dto.Start
            );

            return Ok(new BookingResponseDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                BarberId = booking.BarberId,
                ServiceId = booking.ServiceId,
                Start = booking.Start,
                End = booking.End
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId)
            .ToListAsync();

        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
            return NotFound("Booking not found.");

        return Ok(booking);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
            return NotFound("Booking not found.");

        if (booking.Start <= DateTime.UtcNow)
            return BadRequest("You cannot cancel a booking that has already started or passed.");

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return Ok("Booking cancelled successfully.");
    }
}
