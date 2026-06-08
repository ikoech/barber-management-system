using Microsoft.AspNetCore.Mvc;
using BarberManagementSystem.Models;
using BarberManagementSystem.DTOs.Auth;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BarberManagementSystem.Configuration;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/controller")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthController(AppDbContext context, JwtSettings jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterDto dto)
    {
        if(_context.Users.Any(u => u.Email == dto.Email))
        {
            return BadRequest("Email already exists.");
        }
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
        if (user == null) return Unauthorized("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

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
}
