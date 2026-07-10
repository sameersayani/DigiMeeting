using DigiMeeting.API.Models;

namespace DigiMeeting.API.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int id);
    Task AddAsync(Booking booking);
    Task<bool> HasOverlapAsync(int roomId, DateTime start, DateTime end);
}
