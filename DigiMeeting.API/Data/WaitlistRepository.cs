using DigiMeeting.API.Interfaces;
using DigiMeeting.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiMeeting.API.Data;

public class WaitlistRepository : IWaitlistRepository
{
    private readonly SchedulerDbContext _context;

    public WaitlistRepository(SchedulerDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(WaitingList waitingList)
    {
        await _context.Waitlists.AddAsync(waitingList);
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
}
