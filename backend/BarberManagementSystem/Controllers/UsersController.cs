using BarberManagementSystem.DTOs.Users;
using BarberManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarberManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // ⭐ Only admins can view users
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService)
    {
        _usersService = usersService;
    }

    // GET ALL USERS
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _usersService.GetAllAsync();
        return Ok(result);
    }

    // GET USER BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _usersService.GetByIdAsync(id);
        if (result == null)
            return NotFound("User not found.");

        return Ok(result);
    }
}
