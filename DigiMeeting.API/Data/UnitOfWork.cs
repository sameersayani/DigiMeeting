using DigiMeeting.API.Interfaces;
using DigiMeeting.API.Models;

namespace DigiMeeting.API.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly SchedulerDbContext _context;

    public UnitOfWork(SchedulerDbContext context)
    {
        _context = context;
        Bookings = new BookingRepository(_context); 
        Waitlists = new WaitlistRepository(_context);
        Users = new UserRepository(_context);
        Teams = new BaseRepository<Team>(_context);
        Rooms =  new BaseRepository<MeetingRoom>(_context);
    }

     public IBookingRepository Bookings { get; }
    public IWaitlistRepository Waitlists { get; }
    public IUserRepository Users { get; }

    public IBaseRepository<MeetingRoom> Rooms  { get; }

    public IBaseRepository<Team> Teams  { get; }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
