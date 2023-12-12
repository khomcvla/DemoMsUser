using DemoMsUser.Common.Responses;
using DemoMsUser.Dtos;

namespace DemoMsUser.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResponse> ValidateUsersExist(IEnumerable<string> userIds);
        Task<ServiceResponse> GetUsersCount();
        Task<ServiceResponse> GetAll();
        Task<ServiceResponse> GetUser(string? userId, string? username, string? email);
        Task<ServiceResponse> GetUsersBySubstring(string? username, string? email);

        Task<ServiceResponse> AddUser(string userId, UserPostDto userDto);
        Task<ServiceResponse> AddUsers(List<UserPostDto> userDtos);

        Task<ServiceResponse> UpdateUser(string userId, UserPatchDto userDto);
        Task<ServiceResponse> UpdateUsers(List<UserPatchDtoAdmin> userDtos);

        Task<ServiceResponse> DeleteUser(string userId);
        Task<ServiceResponse> DeleteUsers(List<string> userIds);

        Task<ServiceResponse> SoftDeleteUser(string userId);
    }
}
