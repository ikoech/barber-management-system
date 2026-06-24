using BarberManagementSystem.DTOs.WorkingHours;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkingHoursController : ControllerBase
{
    private readonly WorkingHoursService _workingHoursService;
    private readonly WorkingHoursAvailabilityService _availabilityService;

    public WorkingHoursController(
        WorkingHoursService workingHoursService,
        WorkingHoursAvailabilityService availabilityService)
    {
        _workingHoursService = workingHoursService;
        _availabilityService = availabilityService;
    }

    // Required by booking flow: get availability for a specific date.
    // GET /api/workinghours/barber/{id}?date=YYYY-MM-DD&serviceId=123&stepMinutes=15
    [HttpGet("barber/{barberId}")]
    [Authorize(Roles = "Admin,Barber,Customer")]
    public async Task<IActionResult> GetForDate(
        int barberId,
        [FromQuery] string date,
        [FromQuery] int serviceId,
        [FromQuery] int stepMinutes = 15)
    {
        try
        {
            // Never return 500 and never let invalid inputs crash availability.
            if (barberId <= 0 || serviceId <= 0 || stepMinutes <= 0)
                return Ok(new AvailableTimesResponseDto { isWorking = false });

            if (string.IsNullOrWhiteSpace(date) || !DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
                return Ok(new AvailableTimesResponseDto { isWorking = false });

            var result = await _availabilityService.GetAvailabilityAsync(barberId, parsedDate, serviceId, stepMinutes);
            // Always return 200 OK with a valid DTO.
            return Ok(result ?? new AvailableTimesResponseDto { isWorking = false });
        }
        catch
        {
            return Ok(new AvailableTimesResponseDto
            {
                isWorking = false,
                workingHours = new List<WorkingHourRangeDto>(),
                breaks = new List<TimeRangeDto>(),
                availableTimes = new List<string>(),
                daysOff = new List<string>()
            });
        }
    }



    // Backwards-compatible CRUD endpoints
    [HttpGet("{barberId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetForFrontend(int barberId)
    {
        var result = await _workingHoursService.GetByBarberAsync(barberId);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _workingHoursService.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Barber")]
    public async Task<IActionResult> Create(CreateWorkingHoursDto dto)
    {
        var result = await _workingHoursService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Barber")]
    public async Task<IActionResult> Update(int id, UpdateWorkingHoursDto dto)
    {
        var result = await _workingHoursService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Barber")]
    public async Task<IActionResult> Delete(int id)
    {
        await _workingHoursService.DeleteAsync(id);
        return Ok(new { message = "Working hours deactivated successfully." });
    }
}

