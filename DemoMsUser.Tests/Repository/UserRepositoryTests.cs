using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using DemoMsUser.Data.Models;
using DemoMsUser.Interfaces;
using Newtonsoft.Json;
using Xunit.Abstractions;
using static DemoMsUser.Tests.TestDataHelper;

namespace DemoMsUser.Tests.Repository;

public class UserRepositoryTests
{
    private readonly ITestOutputHelper _console;
    private readonly IUserRepository _userRepository;

    public UserRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        _console = testOutputHelper;
        _userRepository = GetTestUserRepository().GetAwaiter().GetResult();
    }

    private async Task<User?> GetUserById(int id) =>
        await _userRepository.GetUser(GetUserIdString(id), null, null);

    private void Console_WriteLine(object @object) =>
        _console.WriteLine(JsonConvert.SerializeObject(@object, Formatting.Indented));

    //-------------------------------------------------------------------------
    [Fact]
    public async void UserRepository_GetAll_Returns_ListUser()
    {
        //Act
        var users = await _userRepository.GetAll();

        //Assert
        users.Should().NotBeNull();
        users.Should().BeOfType<List<User>>();
        users.Should().HaveCount(MAX_USER_COUNT);
        users.Should().OnlyHaveUniqueItems();
    }

    //-------------------------------------------------------------------------
    [Theory]
    [MemberData(nameof(GetUserMemberData), MemberType = typeof(TestDataHelper))]
    public async void UserRepository_GetUser_Returns_User(
        string? userId,
        string? username,
        string? email,
        bool expectedResult)
    {
        //Act
        var user = await _userRepository.GetUser(userId, username, email);

        //Assert
        expectedResult.Should().Be(user is not null);
    }

    //-------------------------------------------------------------------------
    [Theory]
    [MemberData(nameof(GetSubstringMemberData), MemberType = typeof(TestDataHelper))]
    public async void UserRepository_GetUserBySubstring_Returns_ListUser(
        string? username,
        string? email,
        bool expectedResult)
    {
        //Arrange
        var testUsers = GetAllTestUsers();

        //Act
        var users = await _userRepository.GetUsersBySubstring(username, email);

        //Assert
        users.Should().NotBeNull();
        users.Should().BeOfType<List<User>>();
        users.Should().OnlyHaveUniqueItems();
        users.Should().HaveCount(expectedResult ? MAX_USER_COUNT : ZERO);
        users.Should().BeEquivalentTo(expectedResult ? testUsers : new List<User>());
    }

    //-------------------------------------------------------------------------
    [Theory]
    [MemberData(nameof(GetUserMemberData), MemberType = typeof(TestDataHelper))]
    public async void UserRepository_IsUserExist_Returns_Bool(
        string? userId,
        string? username,
        string? email,
        bool expectedResult)
    {
        //Act
        var result = await _userRepository.IsUserExist(userId, username, email);

        //Assert
        result.Should().Be(expectedResult);
    }

    //-------------------------------------------------------------------------
    [Theory]
    [MemberData(nameof(GetUserIdMemberData), MemberType = typeof(TestDataHelper))]
    public async void UserRepository_IsUserIdExist_Returns_Bool(
        string userId,
        bool expectedResult)
    {
        //Act
        var userExist = await _userRepository.IsUserIdExist(userId);

        //Assert
        userExist.Should().Be(expectedResult);
    }

    //-------------------------------------------------------------------------
    [Theory]
    [MemberData(nameof(GetUsernameMemberData), MemberType = typeof(TestDataHelper))]
    public async void UserRepository_IsUsernameExist_Returns_Bool(
        string username,
        bool expectedResult)
    {
        //Act
        var usernameExist = await _userRepository.IsUsernameExist(username);

        //Assert
        usernameExist.Should().Be(expectedResult);
    }

    //-------------------------------------------------------------------------
    [Theory]
    [MemberData(nameof(GetEmailMemberData), MemberType = typeof(TestDataHelper))]
    public async void UserRepository_IsEmailExist_Returns_Bool(
        string email,
        bool expectedResult)
    {
        //Act
        var emailExist = await _userRepository.IsEmailExist(email);

        //Assert
        emailExist.Should().Be(expectedResult);
    }

    //-------------------------------------------------------------------------
    [Fact]
    public async void UserRepository_Add_And_AddRange_Returns_Bool()
    {
        //Arrange
        var existedUser = GetTestUser(EXISTED_INDEX);
        var newUsers = GetRangeTestUsers(NOT_EXISTED_INDEX, NOT_EXISTED_INDEX + 2);

        //Act
        var countBefore = await _userRepository.GetUsersCount();
        var addResult = await _userRepository.Add(newUsers[0]);
        var addRangeResult = await _userRepository.AddRange(new List<User>(newUsers.Skip(1)));
        var countAfter = await _userRepository.GetUsersCount();

        //Assert
        var addAction = async () => await _userRepository.Add(existedUser);
        await Assert.ThrowsAsync<InvalidOperationException>(addAction);

        addResult.Should().BeTrue();
        addRangeResult.Should().BeTrue();

        countBefore.Should().BeLessThan(countAfter);
        Assert.Equal(3, countAfter - countBefore);
    }

    //-------------------------------------------------------------------------
    [Fact]
    public async void UserRepository_Update_And_UpdateRange_Returns_Bool()
    {
        //Arrange
        var notExistedUser = GetTestUser(NOT_EXISTED_INDEX);
        //NOTE: context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        // var existedUsers = GetRangeTestUsers(FIRST_INDEX, FIRST_INDEX + 2);
        // var (user1, user2, user3) = (existedUsers[0], existedUsers[1], existedUsers[2]);
        var (user1, user2, user3) = (await GetUserById(1), await GetUserById(2), await GetUserById(3));

        //Act
        user1.Email = user2.Email = user3.Email = UPDATED_EMAIL;

        var updateResult = await _userRepository.Update(user1);
        var updateRangeResult = await _userRepository.UpdateRange(new List<User> { user2, user3 });

        //Assert
        var updateAction = async () => await _userRepository.Update(notExistedUser);
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(updateAction);

        updateResult.Should().BeTrue();
        updateRangeResult.Should().BeTrue();

        (await GetUserById(1))!.Email.Should().NotBeNull()
            .And.BeEquivalentTo((await GetUserById(2))!.Email)
            .And.BeEquivalentTo((await GetUserById(3))!.Email)
            .And.BeEquivalentTo(UPDATED_EMAIL);
    }

    //-------------------------------------------------------------------------
    [Fact]
    public async void UserRepository_Delete_And_DeleteRange_Returns_Bool()
    {
        //Arrange
        const int USER_DELETED_COUNT = 3;
        var notExistedUser = GetTestUser(NOT_EXISTED_INDEX);

        //NOTE: context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        // var existedUsers = GetRangeTestUsers(FIRST_INDEX, FIRST_INDEX + 2);
        // var (user1, user2, user3) = (existedUsers[0], existedUsers[1], existedUsers[2]);

        var (user1, user2, user3) = (await GetUserById(1), await GetUserById(2), await GetUserById(3));

        //Act
        var countBefore = await _userRepository.GetUsersCount();
        var deleteResult = await _userRepository.Delete(user1);
        var deleteRangeResult = await _userRepository.DeleteRange(new List<User> { user2, user3 });
        var countAfter = await _userRepository.GetUsersCount();

        //Assert
        var deleteAction = async () => await _userRepository.Delete(notExistedUser);
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(deleteAction);

        countBefore.Should().Be(MAX_USER_COUNT);
        countAfter.Should().Be(MAX_USER_COUNT - USER_DELETED_COUNT);

        deleteResult.Should().BeTrue();
        deleteRangeResult.Should().BeTrue();
    }

    //-------------------------------------------------------------------------
    [Fact]
    public async void UserRepository_SoftDelete_And_SoftDeleteRange_Returns_Bool()
    {
        //Arrange

        //Act

        //Assert
    }
}
