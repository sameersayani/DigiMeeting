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

        // Create a new meeting booking
        public async Task<int> CreateMeetingAsync(string meetingName, string dateTime, int roomId, int teamId)
        {
            var booking = new Booking
            {
                Agenda = meetingName,
                StartTime = DateTime.Parse(dateTime),
                EndTime = DateTime.Parse(dateTime).AddHours(1), // Assuming 1-hour meetings by default
                RoomId = roomId,
                TeamId = teamId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking.Id;
        }

        // Cancel an existing meeting
        public async Task<int> CancelMeetingAsync(int meetingId)
        {
            var booking = await _context.Bookings.FindAsync(meetingId);
            if (booking == null)
            {
                throw new ArgumentException("Meeting not found");
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        
        return booking.Id;
    }

    public Task<int> AddRoomAsync(string roomName, int capacity)
    {
        var room = new MeetingRoom
        {
            Name = roomName,
            Capacity = capacity
        };

        _context.Rooms.Add(room);
        return _context.SaveChangesAsync();
    }

    public Task<int> AddTeamAsync(string teamName, int teamSize, List<string> memberEmailIds)
    {
        var team = new Team
        {
            Name = teamName,
            MemberCount = teamSize,
            Email = memberEmailIds
        };

        _context.Teams.Add(team);
        return _context.SaveChangesAsync();
    }
}
