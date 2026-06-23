using BarberManagementSystem.DTOs.Services;
using BarberManagementSystem.Models;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberManagementSystem.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase   
{
    private readonly AdminBookingService _bookingService;
    private readonly AppDbContext _context;
    private readonly AdminStatsService _statsService;
    public AdminController(AdminBookingService bookingService, AppDbContext context, AdminStatsService statsService)
    {
        _bookingService = bookingService;
        _context = context;
        _statsService = statsService;
    }
    // GET: api/admin/bookings
    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings(
        [FromQuery] int? barberId,
        [FromQuery] int? userId,
        [FromQuery] int? serviceId,
        [FromQuery] DateTime? date)
    {
        var bookings = await _bookingService.GetOverviewAsync(
                barberId, userId, serviceId, date);

        return Ok(bookings);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    { 
        var stats = await _statsService.GetStatsAsync();
        return Ok(stats);
    }

    // PUT: api/admin/users/{userId}/role
    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> SetUserRole(int userId, [FromBody] string role)
    {
       var user = await _context.Users.FindAsync(userId);
       if (user == null) return NotFound("User not found.");

       user.Role = role;
       await _context.SaveChangesAsync();

        return NoContent();
    }
}
