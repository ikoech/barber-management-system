using BarberManagementSystem.DTOs.DayOff;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DaysOffController : ControllerBase
{
    private readonly DaysOffService _daysOffService;

    public DaysOffController(DaysOffService daysOffService)
    {
        _daysOffService = daysOffService;
    }

    // CREATE DAY OFF
    [HttpPost]
    [Authorize(Policy = "BarberOrAdmin")]
    public async Task<IActionResult> CreateDayOff(CreateDayOffDto dto)
    {
        try
        {
            var result = await _daysOffService.CreateDayOffAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // GET ALL DAYS OFF FOR A BARBER
    [HttpGet("barber/{barberId}")]
    [Authorize(Policy = "BarberOrAdmin")]
    public async Task<IActionResult> GetDaysOffForBarber(int barberId)
    {
        var result = await _daysOffService.GetDaysOffForBarberAsync(barberId);
        return Ok(result);
    }

    // DELETE DAY OFF
    [HttpDelete("{id}")]
    [Authorize(Policy = "BarberOrAdmin")]
    public async Task<IActionResult> DeleteDayOff(int id)
    {
        try
        {
            var barberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _daysOffService.DeleteDayOffAsync(id, barberId);

            return Ok(new { message = "Day off removed successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
