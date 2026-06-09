using BarberManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminSetupController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminSetupController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/adminsetup/create
    [HttpPost("create")]
    public async Task<IActionResult> CreateAdmin()
    {
        // Check if an Admin already exists
        if (await _context.Users.AnyAsync(u => u.Role == "Admin"))
            return BadRequest("Admin already exists.");

        var admin = new User
        {
            FullName = "System Admin",
            Email = "admin@barbers.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin1234!"),
            Role = "Admin"
        };

        _context.Users.Add(admin);
        await _context.SaveChangesAsync();

        return Ok("Admin user created successfully.");
    }
}
