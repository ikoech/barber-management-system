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

    public WorkingHoursController(WorkingHoursService workingHoursService)
    {
        _workingHoursService = workingHoursService;
    }

    // ⭐ PUBLIC ENDPOINT FOR FRONTEND (NO AUTH)
    [HttpGet("{barberId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetForFrontend(int barberId)
    {
        var result = await _workingHoursService.GetByBarberAsync(barberId);
        return Ok(result);
    }

    // ⭐ ADMIN‑ONLY CRUD BELOW
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _workingHoursService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("barber/{barberId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByBarber(int barberId)
    {
        var result = await _workingHoursService.GetByBarberAsync(barberId);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateWorkingHoursDto dto)
    {
        var result = await _workingHoursService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateWorkingHoursDto dto)
    {
        var result = await _workingHoursService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _workingHoursService.DeleteAsync(id);
        return Ok(new { message = "Working hours deactivated successfully." });
    }
}
