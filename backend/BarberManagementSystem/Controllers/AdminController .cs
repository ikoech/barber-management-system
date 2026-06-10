using BarberManagementSystem.DTOs.Services;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase   
{
    private readonly AdminBookingService _bookingService;
    public AdminController(AdminBookingService bookingService)
    {
        _bookingService = bookingService;
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
}
