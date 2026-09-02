using DigiMeeting.API.Interfaces;
using DigiMeeting.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiMeeting.API.Data;

public class WaitlistRepository : BaseRepository<WaitingList>, IWaitlistRepository
{
    private readonly SchedulerDbContext _context;

    public WaitlistRepository(SchedulerDbContext context): base(context)
    {
        _context = context;
    }

 
    public async Task<WaitingList?> GetNextTeamForSlotAsync(int capacity, DateTime start, DateTime end)
    {
        return await _context.Waitlists
            .Where(w => w.Status == "Active"
                && w.RequiredCapacity <= capacity
                && w.TargetStartTime == start
                && w.TargetEndTime == end)
            .OrderBy(w => w.CreatedAt)
            .Include(w => w.Team)
            .FirstOrDefaultAsync();
    }

    public Task<int> JoinWaitlist(int teamId, int requiredCapacity, 
    DateTime targetStartTime, DateTime targetEndTime)
    {
        var waitlistEntry = new WaitingList
        {
            TeamId = teamId,
            RequiredCapacity = requiredCapacity,
            TargetStartTime = targetStartTime,
            TargetEndTime = targetEndTime,
            Status = "Active"
        };

        _context.Waitlists.Add(waitlistEntry);
        return _context.SaveChangesAsync();
    }
}
