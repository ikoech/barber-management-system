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

[HttpGet("barber/{barberId}")]
[Authorize(Roles = "Admin,Barber,Customer")]
public async Task<IActionResult> GetForDate(
    int barberId,
    [FromQuery] string? date,
    [FromQuery] int serviceId,
    [FromQuery] int stepMinutes = 15)
{
    try
    {
        if (barberId <= 0)
            return Ok(new AvailableTimesResponseDto { isWorking = false });

        //  FIX: When date is missing → return the barber's working hours
        if (string.IsNullOrWhiteSpace(date))
        {
            var hours = await _workingHoursService.GetByBarberAsync(barberId);

            return Ok(new AvailableTimesResponseDto
            {
                isWorking = hours.Any(),
                workingHours = hours.Select(h => new WorkingHourRangeDto
                {
                    start = h.StartTime,
                    end = h.EndTime
                }).ToList(),
                breaks = new List<TimeRangeDto>(),
                daysOff = new List<string>(),
                availableTimes = new List<string>()
            });
        }

        // If date is invalid → return working hours instead of empty
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
        {
            var hours = await _workingHoursService.GetByBarberAsync(barberId);

            return Ok(new AvailableTimesResponseDto
            {
                isWorking = hours.Any(),
                workingHours = hours.Select(h => new WorkingHourRangeDto
                {
                    start = h.StartTime,
                    end = h.EndTime
                }).ToList(),
                breaks = new List<TimeRangeDto>(),
                daysOff = new List<string>(),
                availableTimes = new List<string>()
            });
        }

        // Normal availability flow
        var result = await _availabilityService.GetAvailabilityAsync(
            barberId, parsedDate, serviceId, stepMinutes);

        return Ok(result ?? new AvailableTimesResponseDto { isWorking = false });
    }
    catch
    {
        return Ok(new AvailableTimesResponseDto { isWorking = false });
    }
}

    // PUBLIC GET for frontend
    [HttpGet("{barberId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetForFrontend(int barberId)
    {
        var result = await _workingHoursService.GetByBarberAsync(barberId);
        return Ok(result);
    }

    // ADMIN GET ALL
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _workingHoursService.GetAllAsync();
        return Ok(result);
    }

    // CREATE (with numeric or string DayOfWeek)
    [HttpPost]
    [Authorize(Roles = "Admin,Barber")]
    public async Task<IActionResult> Create([FromBody] CreateWorkingHoursDto dto)
    {
        if (dto == null)
            return BadRequest(new { message = "Missing working hours payload." });

        if (dto.BarberId <= 0)
            return BadRequest(new { message = "Invalid barberId." });

        // Convert numeric dayOfWeek → string
        string dayString;
        if (int.TryParse(dto.DayOfWeek, out var dayNum))
        {
            if (dayNum < 0 || dayNum > 6)
                return BadRequest(new { message = "DayOfWeek must be 0–6." });

            dayString = ((DayOfWeek)dayNum).ToString();
        }
        else
        {
            dayString = dto.DayOfWeek;
        }

        // Validate times
        if (!TimeOnly.TryParse(dto.StartTime, out var start))
            return BadRequest(new { message = "Invalid StartTime format. Use HH:mm." });

        if (!TimeOnly.TryParse(dto.EndTime, out var end))
            return BadRequest(new { message = "Invalid EndTime format. Use HH:mm." });

        if (start >= end)
            return BadRequest(new { message = "StartTime must be earlier than EndTime." });

        try
        {
            var result = await _workingHoursService.CreateAsync(new CreateWorkingHoursDto
            {
                BarberId = dto.BarberId,
                DayOfWeek = dayString,
                StartTime = start.ToString("HH:mm"),
                EndTime = end.ToString("HH:mm")
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // UPDATE
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Barber")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkingHoursDto dto)
    {
        if (dto == null)
            return BadRequest(new { message = "Missing working hours payload." });

        if (id <= 0)
            return BadRequest(new { message = "Invalid working hours id." });

        if (string.IsNullOrWhiteSpace(dto.DayOfWeek))
            return BadRequest(new { message = "DayOfWeek is required." });

        if (!Enum.TryParse<DayOfWeek>(dto.DayOfWeek.Trim(), true, out _))
            return BadRequest(new { message = "Invalid DayOfWeek. Use Monday..Sunday." });

        if (!TimeOnly.TryParse(dto.StartTime, out var start))
            return BadRequest(new { message = "Invalid StartTime format. Use HH:mm." });

        if (!TimeOnly.TryParse(dto.EndTime, out var end))
            return BadRequest(new { message = "Invalid EndTime format. Use HH:mm." });

        if (start >= end)
            return BadRequest(new { message = "StartTime must be earlier than EndTime." });

        try
        {
            var result = await _workingHoursService.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE (soft delete)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Barber")]
    public async Task<IActionResult> Delete(int id)
    {
        await _workingHoursService.DeleteAsync(id);
        return Ok(new { message = "Working hours deactivated successfully." });
    }
}
