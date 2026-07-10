using DigiMeeting.API.Interfaces;
using DigiMeeting.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiMeeting.API.Data;

public class BookingRepository : IBookingRepository
{
    private readonly SchedulerDbContext _context;

    public BookingRepository(SchedulerDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
    }

    public async Task<bool> HasOverlapAsync(int roomId, DateTime start, DateTime end)
    {
        return await _context.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            !b.IsCancelled &&
            b.StartTime < end &&
            start < b.EndTime);
    }
}
