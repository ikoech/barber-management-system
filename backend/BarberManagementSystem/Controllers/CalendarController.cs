using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Barber")]
public class CalendarController : ControllerBase
{
    private readonly CalendarService _service;

    public CalendarController(CalendarService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetCalendar(int barberId, DateOnly month)
    {
        var result = await _service.GetCalendarAsync(barberId, month);
        return Ok(result);
    }
}
