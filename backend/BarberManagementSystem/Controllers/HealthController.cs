using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Check()
    {
        return Ok(new { status = "Backend is running", timestamp = DateTime.UtcNow });
    }
}