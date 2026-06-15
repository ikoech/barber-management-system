using BarberManagementSystem.DTOs.WorkingHours;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class WorkingHoursController : ControllerBase
{
    private readonly WorkingHoursService _workingHoursService;

    public WorkingHoursController(WorkingHoursService workingHoursService)
    {
        _workingHoursService = workingHoursService;
    }

    // GET ALL WORKING HRS
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _workingHoursService.GetAllAsync();
        return Ok(result);
    }

    // GET BY ID
    [HttpGet("barber/{barberId}")]
    public async Task<IActionResult> GetByBarber(int barberId)
    {
        var result = await _workingHoursService.GetByBarberAsync(barberId);
        return Ok(result);
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkingHoursDto dto)
    {
        var result = await _workingHoursService.CreateAsync(dto);
        return Ok(result);
    }

    // UPDATE
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateWorkingHoursDto dto)
    {
        var result = await _workingHoursService.UpdateAsync(id, dto);
        return Ok(result);
    }

    // DELETE WRK HOURS
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _workingHoursService.DeleteAsync(id);
        return Ok(new { message = "Working hours deactivated successfully." });
    }
}
