using DigiMeeting.API.Models;

namespace DigiMeeting.API.Interfaces;

public interface IBookingRepository: IBaseRepository<Booking>
{
    Task<int> CreateMeetingAsync(string meetingName, string dateTime, int roomId, int teamId);
    Task<bool> HasOverlapAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeBookingId = null);
    Task<int> CancelMeetingAsync(int meetingId);
    Task<int> AddRoomAsync(string roomName, int capacity);
    Task<int> AddTeamAsync(string teamName, int teamSize, List<string> memberEmailIds);
}
