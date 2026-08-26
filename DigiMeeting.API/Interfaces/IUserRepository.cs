using DigiMeeting.API.Models;

namespace DigiMeeting.API.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByAuth0IdAsync(string auth0Id);
    Task<User?> GetByEmailAsync(string email);
}
