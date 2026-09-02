using DigiMeeting.API.Models;

namespace DigiMeeting.API.Interfaces;

public interface IWaitlistRepository: IBaseRepository<WaitingList>
{
    Task<int> JoinWaitlist(int teamId, int requiredCapacity, 
    DateTime targetStartTime, DateTime targetEndTime);
    Task<WaitingList?> GetNextTeamForSlotAsync(int capacity, DateTime start, DateTime end);
}
