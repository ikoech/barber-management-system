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

    //  CREATE BOOKING
    public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto)
    {
        // Validate user exists
        var user = await _context.Users.FindAsync(dto.UserId)
            ?? throw new Exception("User not found.");

        // Validate service exists
        var service = await _context.Services.FindAsync(dto.ServiceId)
            ?? throw new Exception("Service not found.");

        // Validate barber exists
        var barber = await _context.Barbers.FindAsync(dto.BarberId)
            ?? throw new Exception("Barber not found.");

        // Delegate booking creation logic to BookingEngine
        var booking = await _engine.CreateBookingAsync(
            dto.UserId,
            dto.BarberId,
            dto.ServiceId,
            dto.Start
        );

        return new BookingResponseDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            BarberId = booking.BarberId,
            ServiceId = booking.ServiceId,
            Start = booking.Start,
            End = booking.End
        };
    }

    //  GET BOOKINGS FOR USER
    public async Task<List<BookingResponseDto>> GetBookingsForUserAsync(int userId)
    {
        return await _context.Bookings
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.Start)
            .Select(b => new BookingResponseDto
            {
                Id = b.Id,
                BarberId = b.BarberId,
                UserId = b.UserId,
                ServiceId = b.ServiceId,
                Start = b.Start,
                End = b.End
            })
            .ToListAsync();
    }

    //  GET BOOKINGS FOR BARBER
    public async Task<List<BookingResponseDto>> GetBookingsForBarberAsync(int barberId)
    {
        return await _context.Bookings
            .Where(b => b.BarberId == barberId)
            .OrderBy(b => b.Start)
            .Select(b => new BookingResponseDto
            {
                Id = b.Id,
                BarberId = b.BarberId,
                UserId = b.UserId,
                ServiceId = b.ServiceId,
                Start = b.Start,
                End = b.End
            })
            .ToListAsync();
    }

    //  GET BOOKING BY ID
    public async Task<BookingResponseDto> GetBookingByIdAsync(int bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId)
            ?? throw new Exception("Booking not found.");

        return new BookingResponseDto
        {
            Id = booking.Id,
            BarberId = booking.BarberId,
            UserId = booking.UserId,
            ServiceId = booking.ServiceId,
            Start = booking.Start,
            End = booking.End
        };
    }

    //  CANCEL BOOKING
    public async Task<bool> CancelBookingAsync(int bookingId, int userId, bool isAdmin)
    {
        var booking = await _context.Bookings.FindAsync(bookingId)
            ?? throw new Exception("Booking not found.");

        // Only admin or owner can cancel
        if (!isAdmin && booking.UserId != userId)
            throw new Exception("You are not allowed to cancel this booking.");

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();

        return true;
    }
}
