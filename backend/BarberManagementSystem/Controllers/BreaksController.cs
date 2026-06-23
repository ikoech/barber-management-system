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
    public async Task<IActionResult> Create(CreateBreakDto dto)
    {
        var result = await _breakService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateBreakDto dto)
    {
        var result = await _breakService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _breakService.DeleteAsync(id);
        return Ok(new { message = "Break deactivated successfully." });
    }
}
