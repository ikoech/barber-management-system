using BarberManagementSystem.Configuration;
using BarberManagementSystem.DTOs.Auth;
using BarberManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthController(AppDbContext context, JwtSettings jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public IActionResult Register(RegisterDto dto)
    {
        if (_context.Users.Any(u => u.Email == dto.Email))
            return BadRequest("Email already exists.");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "Customer"
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("User registered successfully.");
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var user = _context.Users
            .Include(u => u.BarberProfile)
            .FirstOrDefault(u => u.Email == dto.Email);

        if (user == null)
            return Unauthorized("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }
private string GenerateJwtToken(User user)
{
    var claims = new List<Claim>
    {
        // ✔ AuthContext expects THIS exact claim for user.id
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

        // ✔ AuthContext expects THIS exact claim for email
        new Claim(JwtRegisteredClaimNames.Email, user.Email),

        // ✔ AuthContext expects THIS exact claim for role
        new Claim(ClaimTypes.Role, user.Role),

        // ✔ AuthContext expects "fullName" (camelCase)
        new Claim("fullName", user.FullName ?? "")
    };

    // ✔ AuthContext expects "barberId" as a number-like string
    if (user.Role == "Barber" && user.BarberProfile != null)
    {
        claims.Add(new Claim("barberId", user.BarberProfile.Id.ToString()));
    }
    else
    {
        claims.Add(new Claim("barberId", ""));
    }

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
        audience: _jwtSettings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

    // ADMIN: CREATE TEMP ADMIN
    [AllowAnonymous]
    [HttpPost("create-temp-admin")]
    public async Task<IActionResult> CreateTempAdmin([FromServices] AppDbContext context)
    {
        if (await context.Users.AnyAsync(u => u.Role == "Admin"))
            return BadRequest("Admin already exists.");

        var admin = new User
        {
            FullName = "System Admin",
            Email = "admin@system.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Admin"
        };

        context.Users.Add(admin);
        await context.SaveChangesAsync();

        return Ok("Temporary Admin created. Email: admin@system.com, Password: Admin123!");
    }

    // ADMIN: RESET ADMIN PASSWORD
    [AllowAnonymous]
    [HttpPost("reset-admin-password")]
    public async Task<IActionResult> ResetAdminPassword([FromServices] AppDbContext context)
    {
        var admin = await context.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
        if (admin == null)
            return NotFound("No admin found.");

        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
        await context.SaveChangesAsync();

        return Ok("Admin password reset to Admin123!");
    }
}
