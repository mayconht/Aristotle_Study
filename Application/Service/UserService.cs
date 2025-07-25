using Aristotle.Domain.Entities;
using Aristotle.Domain.Interfaces;

namespace Aristotle.Application.Service;

// Here we define the UserService class, which is responsible for handling user-related operations.
// Usually if a business logic is needed, it should be placed here.
// If you need to add more complex operations, you can create a separate class for that purpose.
/// <summary>
/// 
/// </summary>
public class UserService
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userRepository"></param>
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    // Sometimes the amount of asynchronous operations reminds me of callback hell,
    // why async to await to async to await? anyway...
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<User> CreateUserAsync(User user)
    {
       return await _userRepository.AddAsync(user);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<User> UpdateUserAsync(User user)
    {
        return await _userRepository.UpdateAsync(user);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteUserAsync(Guid id)
    {
        return await _userRepository.DeleteAsync(id);
    }
}