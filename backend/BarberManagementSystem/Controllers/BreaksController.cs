using BarberManagementSystem.DTOs.Breaks;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberManagementSystem.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Barber")]
public class BreaksController : ControllerBase
{
    private readonly BreakService _breakService;

    public BreaksController(BreakService breakService)
    {
        _breakService = breakService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _breakService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("barber/{barberId}")]
    public async Task<IActionResult> GetByBarber(int barberId)
    {
        var result = await _breakService.GetByBarberAsync(barberId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBreakDto dto)
    {
        if (dto == null)
            return BadRequest(new { message = "Missing break payload." });

        if (dto.BarberId <= 0)
            return BadRequest(new { message = "Invalid barberId." });

        if (string.IsNullOrWhiteSpace(dto.DayOfWeek))
            return BadRequest(new { message = "DayOfWeek is required." });

        if (!Enum.TryParse<DayOfWeek>(dto.DayOfWeek.Trim(), true, out _))
            return BadRequest(new { message = "Invalid DayOfWeek. Use Monday..Sunday." });

        if (dto.Start == default || dto.End == default)
            return BadRequest(new { message = "Start and end timestamps are required." });

        // Reject zero/invalid intervals early.
        if (dto.End <= dto.Start)
            return BadRequest(new { message = "End must be after Start." });

        try
        {
            var result = await _breakService.CreateAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBreakDto dto)
    {
        if (dto == null)
            return BadRequest(new { message = "Missing break payload." });

        if (id <= 0)
            return BadRequest(new { message = "Invalid break id." });

        if (string.IsNullOrWhiteSpace(dto.DayOfWeek))
            return BadRequest(new { message = "DayOfWeek is required." });

        if (!Enum.TryParse<DayOfWeek>(dto.DayOfWeek.Trim(), true, out _))
            return BadRequest(new { message = "Invalid DayOfWeek. Use Monday..Sunday." });

        if (dto.Start == default || dto.End == default)
            return BadRequest(new { message = "Start and end timestamps are required." });

        if (dto.End <= dto.Start)
            return BadRequest(new { message = "End must be after Start." });

        try
        {
            var result = await _breakService.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _breakService.DeleteAsync(id);
        return Ok(new { message = "Break deactivated successfully." });
    }
}
