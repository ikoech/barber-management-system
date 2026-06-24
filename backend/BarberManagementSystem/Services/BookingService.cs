using BarberManagementSystem.DTOs.Booking;
using BarberManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberManagementSystem.Services;

public class BookingService
{
    private readonly AppDbContext _context;
    private readonly BookingEngine _engine;

    public BookingService(AppDbContext context, BookingEngine engine)
    {
        _context = context;
        _engine = engine;
    }

    // CREATE BOOKING
    public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto)
    {
        // Validate existence
        var user = await _context.Users.FindAsync(dto.UserId)
            ?? throw new Exception("User not found.");

        var barber = await _context.Barbers
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == dto.BarberId)
            ?? throw new Exception("Barber not found.");

        var service = await _context.Services.FindAsync(dto.ServiceId)
            ?? throw new Exception("Service not found.");

        // Create booking using engine
        var booking = await _engine.CreateBookingAsync(
            dto.UserId,
            dto.BarberId,
            dto.ServiceId,
            dto.Start
        );

        // Reload with navigation properties
            booking = await _context.Bookings
            .Include(b => b.Barber)
                .ThenInclude(barber => barber.User)
            .Include(b => b.Service)
            .FirstAsync(b => b.Id == booking.Id);

        if (booking.Barber?.User == null || booking.Service == null)
            throw new Exception("Booking navigation data missing.");

        return new BookingResponseDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            BarberId = booking.BarberId,
            ServiceId = booking.ServiceId,
            Start = booking.Start,
            End = booking.End,
            BarberName = booking.Barber.User.FullName,
            ServiceName = booking.Service.Name
        };
    }

    // GET BOOKINGS FOR USER
    public async Task<List<BookingResponseDto>> GetBookingsForUserAsync(int userId)
    {
        return await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Barber)
                .ThenInclude(barber => barber.User)
            .Include(b => b.Service)
            .OrderBy(b => b.Start)
            .Select(b => new BookingResponseDto
            {
                Id = b.Id,
                UserId = b.UserId,
                BarberId = b.BarberId,
                ServiceId = b.ServiceId,
                Start = b.Start,
                End = b.End,
                BarberName = b.Barber.User.FullName,
                ServiceName = b.Service.Name
            })
            .ToListAsync();
    }

    // GET BOOKINGS FOR BARBER
    public async Task<List<BookingResponseDto>> GetBookingsForBarberAsync(int barberId)
    {
        return await _context.Bookings
            .Where(b => b.BarberId == barberId)
            .Include(b => b.Barber)
            .ThenInclude(barber => barber.User)
            .Include(b => b.Service)
            .OrderBy(b => b.Start)
            .Select(b => new BookingResponseDto
            {
                Id = b.Id,
                UserId = b.UserId,
                BarberId = b.BarberId,
                ServiceId = b.ServiceId,
                Start = b.Start,
                End = b.End,
                BarberName = b.Barber.User.FullName,
                ServiceName = b.Service.Name
            })
            .ToListAsync();
    }

    // GET BOOKING BY ID
    public async Task<BookingResponseDto> GetBookingByIdAsync(int bookingId)
    {
        var booking = await _context.Bookings
            .Include(b => b.Barber)
                .ThenInclude(barber => barber.User)
            .Include(b => b.Service)
            .FirstOrDefaultAsync(b => b.Id == bookingId)
            ?? throw new Exception("Booking not found.");

        return new BookingResponseDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            BarberId = booking.BarberId,
            ServiceId = booking.ServiceId,
            Start = booking.Start,
            End = booking.End,
            BarberName = booking.Barber.User.FullName,
            ServiceName = booking.Service.Name
        };
    }

    // CANCEL BOOKING
    public async Task CancelBookingAsync(int bookingId, int userId, bool isAdmin)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId)
            ?? throw new Exception("Booking not found.");

        if (!isAdmin && booking.UserId != userId)
            throw new Exception("You are not allowed to cancel this booking.");

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
    }
}
