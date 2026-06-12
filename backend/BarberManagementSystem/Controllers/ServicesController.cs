using BarberManagementSystem.DTOs.Services;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly ServiceService _serviceService;

    public ServicesController(ServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    // PUBLIC: GET ALL SERVICES
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _serviceService.GetAllAsync();
        return Ok(result);
    }

    // PUBLIC: GET SERVICE BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _serviceService.GetByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    // ADMIN: CREATE SERVICE
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceDto dto)
    {
        try
        {
            var result = await _serviceService.CreateAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ADMIN: UPDATE SERVICE
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateServiceDto dto)
    {
        try
        {
            var result = await _serviceService.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ADMIN: DELETE SERVICE
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _serviceService.DeleteAsync(id);
            return Ok(new { message = "Service deleted successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
