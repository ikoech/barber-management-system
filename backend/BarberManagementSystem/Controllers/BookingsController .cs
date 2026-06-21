using BarberManagementSystem.DTOs.Booking;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    //  CREATE BOOKING
   // [Authorize(Policy = "CustomerOrAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
    {
        try
        {
            var result = await _bookingService.CreateBookingAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //  GET BOOKINGS FOR USER
    [Authorize(Policy = "CustomerOrAdmin")]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var result = await _bookingService.GetBookingsForUserAsync(userId);
        return Ok(result);
    }

    //  GET BOOKINGS FOR BARBER
    [Authorize(Policy = "BarberOrAdmin")]
    [HttpGet("barber/{barberId}")]
    public async Task<IActionResult> GetByBarber(int barberId)
    {
        var result = await _bookingService.GetBookingsForBarberAsync(barberId);
        return Ok(result);
    }

    //  GET BOOKING BY ID
    [Authorize(Policy = "CustomerOrAdmin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _bookingService.GetBookingByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    //  CANCEL BOOKING
    [Authorize(Policy = "CustomerOrAdmin")]
    [HttpDelete("{bookingId}")]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)!.Value;

            bool isAdmin = role == "Admin";

            await _bookingService.CancelBookingAsync(bookingId, userId, isAdmin);

            return Ok(new { message = "Booking cancelled successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
