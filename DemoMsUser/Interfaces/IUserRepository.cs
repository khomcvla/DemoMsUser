using DemoMsUser.Data.Models;

namespace DemoMsUser.Interfaces
{
    public interface IUserRepository
    {
        Task<int> GetUsersCount();
        Task<List<User>> GetAll();
        Task<User?> GetUser(string? userId, string? username, string? email);
        Task<List<User>> GetUsersBySubstring(string? username, string? email);

        Task<bool> IsUserExist(string? userId, string? username, string? email);
        Task<bool> IsUserIdExist(string userId);
        Task<bool> IsUsernameExist(string username);
        Task<bool> IsEmailExist(string email);

        Task<bool> Add(User user);
        Task<bool> AddRange(List<User> users);
        Task<bool> Update(User user);
        Task<bool> UpdateRange(List<User> users);
        Task<bool> Delete(User user);
        Task<bool> DeleteRange(List<User> users);
        Task<bool> SoftDelete(User user);
        Task<bool> SoftDeleteRange(List<User> users);
        Task<bool> Save();
    }
}
