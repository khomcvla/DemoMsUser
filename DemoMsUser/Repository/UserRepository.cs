using Microsoft.EntityFrameworkCore;
using DemoMsUser.Common.Helpers;
using DemoMsUser.Data;
using DemoMsUser.Data.Models;
using DemoMsUser.Interfaces;

namespace DemoMsUser.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;

        public UserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<int> GetUsersCount()
        {
            return await _dataContext.Users.CountAsync();
        }

        public async Task<List<User>> GetAll()
        {
            return await _dataContext.Users.ToListAsync();
        }

        public async Task<User?> GetUser(string? userId, string? username, string? email)
        {
            if (StringHelper.IsAllNullOrEmpty(userId, username, email))
                return null;

            var query = _dataContext.Users.AsQueryable();

            query = userId != null ? query.Where(u => u.Id == userId) : query;
            query = username != null ? query.Where(u => u.Username == username) : query;
            query = email != null ? query.Where(u => u.Email == email) : query;

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<User>> GetUsersBySubstring(string? username, string? email)
        {
            if (StringHelper.IsAllNullOrEmpty(username, email))
                return new List<User>();

            var query = _dataContext.Users.AsQueryable();

            query = username != null ? query.Where(u => u.Username.ToLower().Contains(username.ToLower())) : query;
            query = email != null ? query.Where(u => u.Email.ToLower().Contains(email.ToLower())) : query;

            return await query.ToListAsync();
        }

        public async Task<bool> IsUserExist(string? userId, string? username, string? email)
        {
            if (StringHelper.IsAllNullOrEmpty(userId, username, email))
                return false;

            var query = _dataContext.Users.AsQueryable();

            query = userId != null ? query.Where(u => u.Id == userId) : query;
            query = username != null ? query.Where(u => u.Username == username) : query;
            query = email != null ? query.Where(u => u.Email == email) : query;

            return await query.AnyAsync();
        }

        public async Task<bool> IsUserIdExist(string userId)
        {
            return await _dataContext.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<bool> IsUsernameExist(string username)
        {
            return await _dataContext.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailExist(string email)
        {
            return await _dataContext.Users.AnyAsync(u => u.Email == email);
        }

        //ADD
        public async Task<bool> Add(User user)
        {
            await _dataContext.AddAsync(user);
            return await Save();
        }

        public async Task<bool> AddRange(List<User> users)
        {
            _dataContext.AddRange(users);
            return await Save();
        }

        //UPDATE
        public async Task<bool> Update(User user)
        {
            _dataContext.Update(user);
            return await Save();
        }

        public async Task<bool> UpdateRange(List<User> users)
        {
            _dataContext.UpdateRange(users);
            return await Save();
        }

        //DELETE
        public async Task<bool> Delete(User user)
        {
            _dataContext.Remove(user);
            return await Save();
        }

        public async Task<bool> DeleteRange(List<User> users)
        {
            _dataContext.RemoveRange(users);
            return await Save();
        }

        //SOFT DELETE
        public async Task<bool> SoftDelete(User user)
        {
            _dataContext.Entry(user).Property("DeletedAt").CurrentValue = DateTime.Now;
            return await Save();
        }

        public async Task<bool> SoftDeleteRange(List<User> users)
        {
            _dataContext.Entry(users).Property("DeletedAt").CurrentValue = DateTime.Now;
            return await Save();
        }

        //SAVE
        public async Task<bool> Save()
        {
            var saved = await _dataContext.SaveChangesAsync();
            return saved > 0;
        }
    }
}
