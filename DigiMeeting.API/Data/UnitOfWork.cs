using DigiMeeting.API.Interfaces;

namespace DigiMeeting.API.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly SchedulerDbContext _context;

    public UnitOfWork(SchedulerDbContext context)
    {
        _context = context;
        Bookings = new BookingRepository(_context);
        Waitlists = new WaitlistRepository(_context);
    }

    public IBookingRepository Bookings { get; }
    public IWaitlistRepository Waitlists { get; }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
