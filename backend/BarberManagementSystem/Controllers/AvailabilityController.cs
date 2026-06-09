using Microsoft.AspNetCore.Mvc;
using BarberManagementSystem.Services;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailabilityController : ControllerBase
{
    private readonly AvailabilityService _service;

    public AvailabilityController(AvailabilityService service)
    {
        _service = service;
    }

    // GET: api/availability?barberId=1&serviceId=3&date=2024-06-10
    [HttpGet]
    public async Task<IActionResult> GetAvailability(
        int barberId,
        int serviceId,
        DateTime date)
    {
        try
        {
            var slots = await _service.GetAvailabilityAsync(barberId, serviceId, date);
            return Ok(slots);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
