using DigiMeeting.API.Interfaces;
using DigiMeeting.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiMeeting.API.Data;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(SchedulerDbContext context) : base(context) { }

    public async Task<User?> GetByAuth0IdAsync(string auth0Id)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Auth0Id == auth0Id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}
