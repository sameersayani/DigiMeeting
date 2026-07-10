namespace DigiMeeting.API.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBookingRepository Bookings { get; }
    IWaitlistRepository Waitlists { get; }
    Task<int> CompleteAsync();
}
