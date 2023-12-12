using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using DemoMsUser.Common.Constants;
using DemoMsUser.Common.Exceptions;
using DemoMsUser.Common.Responses;
using DemoMsUser.Data.Models;
using DemoMsUser.Dtos;
using DemoMsUser.Interfaces;
using DemoMsUser.Services;
using Xunit.Abstractions;
using static DemoMsUser.Tests.TestDataHelper;

namespace DemoMsUser.Tests.Services;

public class UserServiceTests
{
    private readonly ITestOutputHelper _console;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserServiceTests(ITestOutputHelper testOutputHelper)
    {
        _console = testOutputHelper;

        var userRepository = GetTestUserRepository().GetAwaiter().GetResult();
        _mapper = GetMapper();

        _userService = new UserService(_mapper, userRepository);
    }

    //-------------------------------------------------------------------------
    [Fact]
    public async void UserService_GetAll_Returns_ListUserGetShortDto()
    {
        //Arrange
        var testUsers = GetAllTestUsers();
        var testUsersDtos = _mapper.Map<List<UserGetShortDto>>(testUsers);

        //Act
        var response = await _userService.GetAll();

        //Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<ServiceResponse>();

        response.StatusCode.Should().Be(StatusCodes.Status200OK);

        response.Value.Should().BeAssignableTo<IEnumerable<UserGetShortDto>>();
        (response.Value as IEnumerable<UserGetShortDto>).Should().NotBeNullOrEmpty();
        response.Value.Should().BeEquivalentTo(testUsersDtos);
    }

    //-------------------------------------------------------------------------
    [Theory]
    [MemberData(nameof(GetSubstringMemberData), MemberType = typeof(TestDataHelper))]
    public async void UserService_GetUsersBySubstring_Returns_ListUserGetShortDto(
        string? username,
        string? email,
        bool expectedResult)
    {
        try
        {
            //Arrange
            var testUsers = GetAllTestUsers();
            var testUsersDtos = _mapper.Map<List<UserGetShortDto>>(testUsers);

            //Act
            var response = await _userService.GetUsersBySubstring(username, email);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ServiceResponse>();

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            response.Value.Should().BeAssignableTo<IEnumerable<UserGetShortDto>>();
            (response.Value as IEnumerable<UserGetShortDto>).Should().HaveCount(expectedResult ? MAX_USER_COUNT : ZERO);
            response.Value.Should().BeEquivalentTo(expectedResult ? testUsersDtos : new List<UserGetShortDto>());
        }
        catch (Exception ex)
        {
            ex.Should().BeOfType<InvalidInputException>();
        }
    }

    //-------------------------------------------------------------------------
    [Theory]
    [MemberData(nameof(GetUserMemberData), MemberType = typeof(TestDataHelper))]
    public async void UserService_GetUser_Returns_UserGetDto(
        string? userId,
        string? username,
        string? email)
    {
        try
        {
            //Arrange
            var testUser = GetTestUser(EXISTED_INDEX);
            var testUserDto = _mapper.Map<UserGetDto>(testUser);

            //Act
            var response = await _userService.GetUser(userId, username, email);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ServiceResponse>();

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            response.Value.Should().BeOfType<UserGetDto>();
            response.Value.Should().BeEquivalentTo(testUserDto);
        }
        catch (InvalidInputException ex)
        {
            ex.Should().BeOfType<InvalidInputException>();
        }
        catch (UserNotExistException ex)
        {
            ex.Should().BeOfType<UserNotExistException>();
        }
    }

    //-------------------------------------------------------------------------
    [Theory]
    [MemberData(nameof(AddUserMemberData), MemberType = typeof(TestDataHelper))]
    public async void UserService_AddUser_Returns_UserDto(
        string userId,
        UserPostDto userDto)
    {
        try
        {
            //Arrange
            var testUser = GetTestUser(NOT_EXISTED_INDEX);
            var testUserDto = GetMapper().Map<UserPostDto>(testUser);

            //Act
            var response = await _userService.AddUser(userId, userDto);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ServiceResponse>();

            response.StatusCode.Should().Be(StatusCodes.Status201Created);
            response.Value.Should().BeOfType<UserPostDto>();
            response.Value.Should().BeEquivalentTo(testUserDto);
        }
        catch (Exception ex)
        {
            ex.Should().BeOfType<UserAlreadyExistsException>();
        }
    }

    //-------------------------------------------------------------------------
    [Theory]
    [MemberData(nameof(UpdateUserMemberData), MemberType = typeof(TestDataHelper))]
    public async void UserService_UpdateUser_Returns_String(
        string userId,
        UserPatchDto userDto)
    {
        try
        {
            //Act
            var response = await _userService.UpdateUser(userId, userDto);
            var user = await _userService.GetUser(userId, null, null);

            //Assert
            response.Should().NotBeNull();
            response.Should().BeOfType<ServiceResponse>();

            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            response.Value.Should().Be(StatusMessages.StatusSuccess);

            (user.Value as User)?.Email.Should().Be(UPDATED_EMAIL);
        }
        catch (Exception ex)
        {
            ex.Should().BeOfType<UserNotExistException>();
        }
    }

    //-------------------------------------------------------------------------
    [Theory]
    [MemberData(nameof(DeleteUserMemberData), MemberType = typeof(TestDataHelper))]
    public async void UserService_DeleteUser_Returns_String(string userId)
    {
        try
        {
            //Act
            var countBefore = (await _userService.GetUsersCount()).Value;
            var response = await _userService.DeleteUser(userId);
            var countAfter = (await _userService.GetUsersCount()).Value;

            //Assert
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            response.Value.Should().Be(StatusMessages.StatusSuccess);

            countBefore.Should().Be(MAX_USER_COUNT);
            countAfter.Should().Be(MAX_USER_COUNT - 1);
        }
        catch (Exception ex)
        {
            ex.Should().BeOfType<UserNotExistException>();
        }
    }
}
