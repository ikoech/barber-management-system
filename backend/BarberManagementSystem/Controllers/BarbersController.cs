using BarberManagementSystem.DTOs.Barber;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Any logged-in user can reach this controller
public class BarbersController : ControllerBase
{
    private readonly BarberService _barberService;
    private readonly ScheduleService _scheduleService;

    public BarbersController(BarberService barberService, ScheduleService scheduleService)
    {
        _barberService = barberService;
        _scheduleService = scheduleService;
    }

    // ADMIN: GET ALL BARBERS
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _barberService.GetAllAsync();
        return Ok(result);
    }

    // ADMIN: GET BARBER BY ID
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _barberService.GetByIdAsync(id);
        if (result == null)
            return NotFound("Barber not found.");

        return Ok(result);
    }

    // ADMIN: CREATE BARBER
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateBarberDto dto)
    {
        var result = await _barberService.CreateAsync(dto);
        return Ok(result);
    }

    // ADMIN: UPDATE BARBER
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateBarberDto dto)
    {
        try
        {
            var result = await _barberService.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // BARBER + ADMIN: DAILY SCHEDULE
    [Authorize(Policy = "BarberOrAdmin")]
    [HttpGet("{barberId}/schedule")]
    public async Task<IActionResult> GetSchedule(int barberId, [FromQuery] DateTime date)
    {
        var result = await _scheduleService.GetScheduleAsync(barberId, date);
        return Ok(result);
    }

    // BARBER + ADMIN: MONTHLY SCHEDULE
    [Authorize(Policy = "BarberOrAdmin")]
    [HttpGet("{barberId}/schedule/monthly")]
    public async Task<IActionResult> GetMonthlySchedule(int barberId, int year, int month)
    {
        var result = await _scheduleService.GetMonthlyScheduleAsync(barberId, year, month);

        if (result == null)
            return Ok(new { message = "No schedule found", barberId, year, month });

        return Ok(result);
    }

    // ADMIN: DELETE BARBER (SOFT DELETE)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _barberService.DeleteAsync(id);
            return Ok(new { message = "Barber deactivated successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
