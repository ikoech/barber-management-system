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

    // Public (if you still want anonymous read access)
    [HttpGet("{barberId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetForFrontend(int barberId)
    {
        var result = await _workingHoursService.GetByBarberAsync(barberId);
        return Ok(result);
    }

    // Admin-only: list all working hours
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _workingHoursService.GetAllAsync();
        return Ok(result);
    }

    // Barber/Customer/Admin: get working hours for a specific barber
    // Customers are allowed to view working hours for booking.
    [HttpGet("barber/{barberId}")]
    [Authorize(Roles = "Admin,Barber,Customer")]
    public async Task<IActionResult> GetByBarber(int barberId)
    {
        var result = await _workingHoursService.GetByBarberAsync(barberId);
        return Ok(result);
    }

    // Barber or Admin: create working hours
    [HttpPost]
    [Authorize(Roles = "Admin,Barber")]
    public async Task<IActionResult> Create(CreateWorkingHoursDto dto)
    {
        var result = await _workingHoursService.CreateAsync(dto);
        return Ok(result);
    }

    // Barber or Admin: update working hours
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Barber")]
    public async Task<IActionResult> Update(int id, UpdateWorkingHoursDto dto)
    {
        var result = await _workingHoursService.UpdateAsync(id, dto);
        return Ok(result);
    }

    // Barber or Admin: delete working hours
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Barber")]
    public async Task<IActionResult> Delete(int id)
    {
        await _workingHoursService.DeleteAsync(id);
        return Ok(new { message = "Working hours deactivated successfully." });
    }
}
