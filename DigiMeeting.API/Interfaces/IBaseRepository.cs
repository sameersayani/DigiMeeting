using System.Collections;

namespace DigiMeeting.API.Interfaces;

public interface IBaseRepository<T>
{
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T room);
    Task<int> UpdateAsync(T room);
    Task<Boolean> DeleteAsync(int id);
    Task<IList> GetListAsync();
}
