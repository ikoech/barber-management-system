using Microsoft.AspNetCore.Mvc;
namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase  
{
    [HttpGet]
    public IActionResult Check() => Ok("Backend is running");
}
