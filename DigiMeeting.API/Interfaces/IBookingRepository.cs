using DigiMeeting.API.Models;

namespace DigiMeeting.API.Interfaces;

public interface IBookingRepository: IBaseRepository<Booking>
{
    Task<bool> HasOverlapAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
}
