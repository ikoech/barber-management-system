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


    //CREATE BARBER
    public async Task<BarberResponseDto> CreateAsync(CreateBarberDto dto)
    {
        var exists = await _context.Barbers.AnyAsync(b => b.UserId == dto.UserId);
        if (exists)
            throw new Exception("This user is already registered as a barber.");


        var barber = new Barber
        {
            UserId = dto.UserId,
            Specialization = dto.Specialization,
            IsActive = true
        };

        _context.Barbers.Add(barber);
        await _context.SaveChangesAsync();

        return new BarberResponseDto
        {
            Id = dto.UserId,
            UserId = dto.UserId,
            Specialization = dto.Specialization,
            IsActive = barber.IsActive
        };
    }

    // GET ALL ACTIVE BARBERS
    public async Task<List<BarberResponseDto>> GetAllAsync()
    {
        return await _context.Barbers
            .Where(b => b.IsActive)
            .Select(b => new BarberResponseDto
            {
                Id = b.Id,
                UserId = b.UserId,
                Specialization = b.Specialization,
                IsActive = b.IsActive
            })
            .ToListAsync();
    }

    //GET BARBER BY ID
    public async Task<BarberResponseDto?> GetByIdAsync(int id)
    {
        var barber = await _context.Barbers.FindAsync(id);
        if (barber == null)
            return null;

        return new BarberResponseDto
        {
            Id = barber.Id,
            UserId = barber.UserId,
            Specialization = barber.Specialization,
            IsActive = barber.IsActive
        };
    }

    // UPDATE BARBER
    public async Task<BarberResponseDto> UpdateAsync(int id, UpdateBarberDto dto)
    {
        var barber = await _context.Barbers.FindAsync(id)
            ?? throw new ArgumentException("Barber not found");

        barber.Specialization = dto.Specialization;

        await _context.SaveChangesAsync();

        return new BarberResponseDto
        {
            Id = barber.Id,
            UserId = barber.UserId,
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
