using DigiMeeting.API.Models;

namespace DigiMeeting.API.Interfaces;

public interface IWaitlistRepository
{
    Task AddAsync(WaitingList waitingList);
    Task<WaitingList?> GetNextTeamForSlotAsync(int capacity, DateTime start, DateTime end);
}
