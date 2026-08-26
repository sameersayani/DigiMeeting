using DigiMeeting.API.Interfaces;
using DigiMeeting.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiMeeting.API.Data;

public class BookingRepository :  BaseRepository<Booking>, IBookingRepository
{
    private readonly SchedulerDbContext _context;

    public BookingRepository(SchedulerDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> HasOverlapAsync(int roomId, 
                            DateTime startTime, DateTime endTime, 
                            int? excludeBookingId = null)
    {
       var query = _context.Bookings
        .Where(b => b.RoomId == roomId &&
                    b.StartTime < endTime &&
                    b.EndTime > startTime);

        if (excludeBookingId.HasValue)
        {
            query = query.Where(b => b.Id != excludeBookingId.Value);
        }

        return await query.AnyAsync();
    }
}
