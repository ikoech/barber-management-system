using BarberManagementSystem.DTOs.Services;
using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Services;

public class ServiceService
{
    private readonly AppDbContext _context;

    public ServiceService(AppDbContext context)
    {
        _context = context;
    }

    // CREATE SERVICE
    public async Task<ServiceResponseDto> CreateAsync(CreateServiceDto dto)
    {
        var service = new Service
        {
            Name = dto.Name,
            DurationMinutes = dto.DurationMinutes,
            Price = dto.Price,
            IsActive = true
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return new ServiceResponseDto
        {
            Id = service.Id,
            Name = service.Name,
            DurationMinutes = service.DurationMinutes,
            Price = service.Price,
            IsActive = service.IsActive
        };
    }

    // GET ALL ACTIVE SERVICES
    public async Task<List<ServiceResponseDto>> GetAllAsync()
    {
        return await _context.Services
            .Where(s => s.IsActive)
            .Select(s => new ServiceResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                DurationMinutes = s.DurationMinutes,
                Price = s.Price,
                IsActive = s.IsActive
            })
            .ToListAsync();
    }

    // GET SERVICE BY ID
    public async Task<ServiceResponseDto?> GetByIdAsync(int id)
    {
        var service = await _context.Services.FindAsync(id);
        if (service == null)
            return null;

        return new ServiceResponseDto
        {
            Id = service.Id,
            Name = service.Name,
            DurationMinutes = service.DurationMinutes,
            Price = service.Price,
            IsActive = service.IsActive
        };
    }

    // UPDATE SERVICE
    public async Task<ServiceResponseDto?> UpdateAsync(int id, UpdateServiceDto dto)
    {
        var service = await _context.Services.FindAsync(id)
            ?? throw new Exception("Service not found");

        service.Name = dto.Name;
        service.DurationMinutes = dto.DurationMinutes;
        service.Price = dto.Price;

        await _context.SaveChangesAsync();

        return new ServiceResponseDto
        {
            Id = service.Id,
            Name = service.Name,
            DurationMinutes = service.DurationMinutes,
            Price = service.Price,
            IsActive = service.IsActive
        };
    }

    // SOFT DELETE SERVICE
    public async Task<bool> DeleteAsync(int id)
    {
        var service = await _context.Services.FindAsync(id)
            ?? throw new Exception("Service not found");

        // Soft delete instead of removing
        service.IsActive = false;

        await _context.SaveChangesAsync();
        return true;
    }
}
