using DigiMeeting.API.Models;

namespace DigiMeeting.API.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBookingRepository Bookings { get; }
    IWaitlistRepository Waitlists { get; }
    IUserRepository Users { get; }

    IBaseRepository<MeetingRoom> Rooms { get; }
    IBaseRepository<Team> Teams { get; }

    Task<int> CompleteAsync();
}
