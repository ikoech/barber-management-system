using BarberManagementSystem.DTOs.Services;
using BarberManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ServicesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/services
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var services = await _context.Services.ToListAsync();
        return Ok(services);
    }

    // GET: api/services/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return NotFound("Service not found");

        return Ok(service);
    }

    // POST: api/services
    [Authorize(Policy = "AdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceDto dto)
    {
        var service = new Service
        {
            Name = dto.Name,
            DurationMinutes = dto.DurationMinutes,
            Price = dto.Price
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return Ok(service);
    }

    // PUT: api/services/{id}
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateServiceDto dto)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return NotFound("Service not found");

        service.Name = dto.Name;
        service.DurationMinutes = dto.DurationMinutes;
        service.Price = dto.Price;

        await _context.SaveChangesAsync();

        return Ok(service);
    }

    // DELETE: api/services/{id}
    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return NotFound("Service not found");

        _context.Services.Remove(service);
        await _context.SaveChangesAsync();

        return Ok("Service deleted");
    }
}
