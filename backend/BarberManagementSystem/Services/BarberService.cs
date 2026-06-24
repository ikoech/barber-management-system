using BarberManagementSystem.DTOs.Barber;
using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Services;

public class BarberService
{
    private readonly AppDbContext _context;

    public BarberService(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    // CREATE BARBER
    public async Task<BarberResponseDto> CreateAsync(CreateBarberDto dto)
    {
        if (await _context.Barbers.AnyAsync(b => b.UserId == dto.UserId))
            throw new Exception("This user is already registered as a barber.");

        var barber = new Barber
        {
            UserId = dto.UserId,
            Specialization = dto.Specialization,
            IsActive = true
        };

        _context.Barbers.Add(barber);
        await _context.SaveChangesAsync();

        var user = await _context.Users.FindAsync(barber.UserId);

        return new BarberResponseDto
        {
            Id = barber.Id,
            UserId = barber.UserId,
            FullName = user?.FullName ?? "(Unknown Barber)",
            Specialization = barber.Specialization,
            IsActive = barber.IsActive
        };
    }

    // GET ALL ACTIVE BARBERS
    public async Task<List<BarberResponseDto>> GetAllAsync()
    {
        return await _context.Barbers
            .AsNoTracking()
            .Include(b => b.User) // Always load User navigation
            .Where(b => b.IsActive)
            .Select(b => new BarberResponseDto
            {
                Id = b.Id,
                UserId = b.UserId,
                FullName = (b.User != null ? (b.User.FullName ?? "") : "") ?? "(Unknown Barber)",
                Specialization = b.Specialization ?? string.Empty,
                IsActive = b.IsActive
            })
            .ToListAsync();
    }

    // GET BARBER BY ID
    public async Task<BarberResponseDto?> GetByIdAsync(int id)
    {
        var barber = await _context.Barbers
            .AsNoTracking()
            .Include(b => b.User) // Always load User navigation
            .FirstOrDefaultAsync(b => b.Id == id);

        if (barber == null)
            return null;

        return new BarberResponseDto
        {
            Id = barber.Id,
            UserId = barber.UserId,
            FullName = !string.IsNullOrWhiteSpace(barber.User?.FullName)
                ? barber.User.FullName
                : "(Unknown Barber)",
            Specialization = barber.Specialization ?? string.Empty,
            IsActive = barber.IsActive
        };
    }

    // UPDATE BARBER
    public async Task<BarberResponseDto> UpdateAsync(int id, UpdateBarberDto dto)
    {
        var barber = await _context.Barbers
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id)
            ?? throw new ArgumentException("Barber not found");

        barber.Specialization = dto.Specialization;

        await _context.SaveChangesAsync();

        return new BarberResponseDto
        {
            Id = barber.Id,
            UserId = barber.UserId,
            FullName = barber.User != null ? barber.User.FullName : "(Unknown Barber)",
            Specialization = barber.Specialization,
            IsActive = barber.IsActive
        };
    }

    // SOFT DELETE BARBER
    public async Task<bool> DeleteAsync(int id)
    {
        var barber = await _context.Barbers.FindAsync(id)
            ?? throw new Exception("Barber not found");

        barber.IsActive = false;

        await _context.SaveChangesAsync();
        return true;
    }
}
