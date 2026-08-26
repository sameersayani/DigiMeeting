using DigiMeeting.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace DigiMeeting.API.Data;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly SchedulerDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(SchedulerDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IList> GetListAsync()
    {
        return await _dbSet.ToListAsync();
    }
}