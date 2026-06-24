using BarberManagementSystem.DTOs.Booking;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingService _bookingService;

    public BookingsController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    [Authorize(Policy = "CustomerOrAdmin")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        if (dto == null)
            return BadRequest(new { message = "Missing booking payload." });

        if (dto.UserId <= 0)
            return BadRequest(new { message = "Invalid userId." });

        if (dto.ServiceId <= 0)
            return BadRequest(new { message = "Invalid serviceId." });

        if (dto.BarberId <= 0)
            return BadRequest(new { message = "Invalid barberId." });

        // Validate ISO/DateTime
        if (dto.Start == default)
            return BadRequest(new { message = "Invalid start datetime." });

        // Normalize kind to UTC if unspecified
        if (dto.Start.Kind == DateTimeKind.Unspecified)
            dto.Start = DateTime.SpecifyKind(dto.Start, DateTimeKind.Utc);

        try
        {
            var result = await _bookingService.CreateBookingAsync(dto);
            return Created($"/api/bookings/{result.Id}", result);
        }
        catch (Exception ex)
        {
            // Let BookingEngine exceptions surface as 400 so frontend can show correct reason.
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpGet("user/{userId}")]
    [Authorize(Policy = "CustomerOrAdmin")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var result = await _bookingService.GetBookingsForUserAsync(userId);
        return Ok(result);
    }

   
    [HttpGet("barber/{barberId}")]
    [Authorize(Policy = "BarberOrAdmin")]
    public async Task<IActionResult> GetByBarber(int barberId)
    {
        var result = await _bookingService.GetBookingsForBarberAsync(barberId);
        return Ok(result);
    }

    [HttpDelete("{bookingId}")]
    [Authorize(Policy = "CustomerOrAdmin")]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;
            var isAdmin = role == "Admin";

            await _bookingService.CancelBookingAsync(bookingId, userId, isAdmin);
            return Ok(new { message = "Booking cancelled successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
