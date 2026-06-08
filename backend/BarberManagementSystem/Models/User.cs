using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BarberManagementSystem.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]        // Roles: Admin, Barber, Customer
    public string Role { get; set; } = "Customer"; // Default role is Customer

    //Navigation properties
    public Barber? BarberProfile { get; set; }
    public ICollection<Booking>? Bookings { get; set; }
}