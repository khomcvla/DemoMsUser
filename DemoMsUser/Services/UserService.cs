using AutoMapper;
using DemoMsUser.Common.Constants;
using DemoMsUser.Common.Exceptions;
using DemoMsUser.Common.Helpers;
using DemoMsUser.Common.Responses;
using DemoMsUser.Data.Models;
using DemoMsUser.Dtos;
using DemoMsUser.Interfaces;

namespace DemoMsUser.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserService(
            IMapper mapper,
            IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse> ValidateUsersExist(IEnumerable<string> userIds)
        {
            var users = (await _userRepository.GetAll()).Select(u => u.Id).ToList();
            var result = !userIds.Except(users).Any();
            return new ServiceResponse(
                StatusCodes.Status200OK,
                result
                    ? StatusMessages.ValidationSuccess
                    : StatusMessages.ValidationFailed);
        }

        public async Task<ServiceResponse> GetUsersCount()
        {
            var usersCount = await _userRepository.GetUsersCount();
            return new ServiceResponse(StatusCodes.Status200OK, usersCount);
        }

        public async Task<ServiceResponse> GetAll()
        {
            var users = await _userRepository.GetAll();
            var usersDtos = _mapper.Map<List<UserGetShortDto>>(users);
            return new ServiceResponse(StatusCodes.Status200OK, usersDtos);
        }

        public async Task<ServiceResponse> GetUsersBySubstring(string? username, string? email)
        {
            if (StringHelper.IsAllNullOrEmpty(username, email))
                throw new InvalidInputException();

            var users = await _userRepository.GetUsersBySubstring(username, email);
            var usersDtos = _mapper.Map<List<UserGetShortDto>>(users);

            return new ServiceResponse(StatusCodes.Status200OK, usersDtos);
        }

        public async Task<ServiceResponse> GetUser(string? userId, string? username, string? email)
        {
            if (StringHelper.IsAllNullOrEmpty(userId, username, email))
                throw new InvalidInputException();

            var user = await _userRepository.GetUser(userId, username, email);
            if (user == null || (username != null && !username.Equals(user.Username, StringComparison.Ordinal)))
            {
                var userInfo = string.Empty;
                userInfo += userId != null ? $"userId = {userId}; " : userInfo;
                userInfo += username != null ? $"username = {username}; " : userInfo;
                userInfo += email != null ? $"email = {email};" : "";
                throw new UserNotExistException(userInfo);
            }

            var userDto = _mapper.Map<UserGetDto>(user);
            return new ServiceResponse(StatusCodes.Status200OK, userDto);
        }

        private async Task<List<string?>> ReturnExistData(string id, string username, string email)
        {
            return new List<string?>
            {
                await _userRepository.IsUserIdExist(id) ? id + StatusMessages.AlreadyExist : null,
                await _userRepository.IsUsernameExist(username) ? username + StatusMessages.AlreadyExist : null,
                await _userRepository.IsEmailExist(email) ? email + StatusMessages.AlreadyExist : null
            };
        }

        public async Task<ServiceResponse> AddUser(string userId, UserPostDto userDto)
        {
            var exist = new List<string>();
            exist.AddRange(
                (await ReturnExistData(userDto.Id, userDto.Username, userDto.Email)).Where(s => s != null)!);

            if (exist.Any())
                throw new UserAlreadyExistsException(exist);

            var user = _mapper.Map<User>(userDto);
            var result = await _userRepository.Add(user);
            if (result == false) throw new Exception();

            return new ServiceResponse(StatusCodes.Status201Created, userDto);
        }

        public async Task<ServiceResponse> AddUsers(List<UserPostDto> userDtos)
        {
            //check if id, username or email are available
            var exist = new List<string>();

            //Moreover validation dto????
            foreach (var uDto in userDtos)
                exist.AddRange(
                    (await ReturnExistData(uDto.Id, uDto.Username, uDto.Email)).Where(s => s != null)!);

            if (exist.Any())
                throw new UserAlreadyExistsException(exist);

            var users = _mapper.Map<List<User>>(userDtos);
            var result = await _userRepository.AddRange(users);
            if (result == false) throw new Exception();

            return new ServiceResponse(StatusCodes.Status201Created, userDtos);
        }

        public async Task<ServiceResponse> UpdateUser(string userId, UserPatchDto userDto)
        {
            var user = await _userRepository.GetUser(userId, null, null);
            if (user is null)
                throw new UserNotExistException($"{userId} - Doesn't exist in system.");

            _mapper.Map(userDto, user);

            var result = await _userRepository.Update(user);
            if (result == false) throw new Exception();

            return new ServiceResponse(StatusCodes.Status200OK, StatusMessages.StatusSuccess);
        }

        public async Task<ServiceResponse> UpdateUsers(List<UserPatchDtoAdmin> userDtos)
        {
            var usersToUpdate = new List<User>();
            var errors = new List<string>();

            //Foreach to set updated data inside exist user
            foreach (var uDto in userDtos)
            {
                //provide validation of dto datas/maybe it will be somewhere else
                //************************************* VALIDATE DATAS *************************************
                // var resultValidation = Validate();
                // if (!resultValidation)
                // {
                //     errors.Add($"{uDto.Id} - Not correct");
                //     continue;
                // }

                var user = await _userRepository.GetUser(uDto.Id, null, null);
                if (user is null)
                {
                    errors.Add($"{uDto.Id} - Doesn't exist in system");
                    continue;
                }

                _mapper.Map(uDto, user);
                usersToUpdate.Add(user);
            }

            var result = await _userRepository.UpdateRange(usersToUpdate);
            if (result == false) throw new Exception();

            return userDtos.Count - errors.Count > 0
                ? new ServiceResponse(StatusCodes.Status207MultiStatus,
                    new ErrorDetails(
                        $"{userDtos.Count - errors.Count} users updated successfully. Failures occurred with the others.",
                        errors))
                : new ServiceResponse(StatusCodes.Status200OK, "All users updated successfully.");
        }

        public async Task<ServiceResponse> DeleteUser(string userId)
        {
            var userIds = new List<string> { userId };
            return await DeleteUsers(userIds);
        }

        public async Task<ServiceResponse> DeleteUsers(List<string> userIds)
        {
            //check if all users exist
            var usersNotExist = new List<string>();
            foreach (var userId in userIds)
                if (!await _userRepository.IsUserIdExist(userId))
                    usersNotExist.Add(userId);

            if (usersNotExist.Any())
                throw new UserNotExistException(usersNotExist);

            //TODO: We can't delete User that has Id only.
            // User model has non-null "Email" and "Username"
            // so at this moment decided first-of-all to retrieve existed instance with all info based on userId

            // var users = userIds.Select(uId => new User { Id = uId }).ToList();

            //TODO: Think how to improve and replace with the code above
            var users = new List<User>();
            foreach (var uId in userIds)
            {
                var user = await _userRepository.GetUser(uId, null, null);
                if (user is not null)
                    users.Add(user);
            }

            var result = await _userRepository.DeleteRange(users);
            if (result == false) throw new Exception();

            return new ServiceResponse(StatusCodes.Status200OK, StatusMessages.StatusSuccess);
        }

        public async Task<ServiceResponse> SoftDeleteUser(string userId)
        {
            var user = await _userRepository.GetUser(userId, null, null);
            if (user is null) throw new UserNotExistException($"userId = {userId}");

            var result = await _userRepository.SoftDelete(user);
            if (result == false) throw new Exception();

            return new ServiceResponse(StatusCodes.Status200OK, StatusMessages.StatusSuccess);
        }
    }
}
