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
            IsActive = dto.IsActive,
            BarberId = dto.BarberId
        };

        _context.Services.Add(service);
        await _context.SaveChangesAsync();

        return new ServiceResponseDto
        {
            Id = service.Id,
            Name = service.Name,
            DurationMinutes = service.DurationMinutes,
            Price = service.Price,
            IsActive = service.IsActive,
            BarberId = service.BarberId
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
                IsActive = s.IsActive,
                BarberId = s.BarberId
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
            IsActive = service.IsActive,
            BarberId = service.BarberId
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
        service.IsActive = dto.IsActive;
        service.BarberId = dto.BarberId;

        await _context.SaveChangesAsync();

        return new ServiceResponseDto
        {
            Id = service.Id,
            Name = service.Name,
            DurationMinutes = service.DurationMinutes,
            Price = service.Price,
            IsActive = service.IsActive,
            BarberId = service.BarberId
        };
    }

    // SOFT DELETE SERVICE
    public async Task<bool> DeleteAsync(int id)
    {
        var service = await _context.Services.FindAsync(id)
            ?? throw new Exception("Service not found");

        service.IsActive = false;

        await _context.SaveChangesAsync();
        return true;
    }
}
