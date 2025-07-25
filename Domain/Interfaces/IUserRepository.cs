using Aristotle.Domain.Entities;

namespace Aristotle.Domain.Interfaces;

// I truly like to keep interfaces lke this, it is easier to find the data access layer
// and it is eeasier to implement the repository pattern
/// <summary>
/// 
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<User?> GetByIdAsync(Guid id);
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<User>> GetAllAsync();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<User> AddAsync(User user);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<User> UpdateAsync(User user);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Guid id);
}