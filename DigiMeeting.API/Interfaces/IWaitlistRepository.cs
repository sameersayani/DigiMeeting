using DigiMeeting.API.Models;

namespace DigiMeeting.API.Interfaces;

public interface IWaitlistRepository: IBaseRepository<WaitingList>
{
    Task<WaitingList?> GetNextTeamForSlotAsync(int capacity, DateTime start, DateTime end);
}
