using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DemoMsUser.Data;
using DemoMsUser.Data.Models;
using DemoMsUser.Dtos;
using DemoMsUser.Dtos.Profiles;
using DemoMsUser.Repository;

namespace DemoMsUser.Tests;

public static class TestDataHelper
{
    public const int ZERO = 0;

    public const int MAX_USER_COUNT = 5;

    public const int FIRST_INDEX = 1;
    public const int LAST_INDEX = MAX_USER_COUNT;

    public const int EXISTED_INDEX = FIRST_INDEX;
    public const int NOT_EXISTED_INDEX = LAST_INDEX + 1;

    public const string ID_BASE = "Id";
    public const string EMAIL_BASE = "Email";
    public const string UPDATED_EMAIL = $"Updated{EMAIL_BASE}";
    public const string USERNAME_BASE = "Username";

    public const string EMPTY_DATA = "";
    public const string NOT_EXISTED_DATA = "NOT_EXISTED_DATA";

    //-------------------------------------------------------------------------
    public static string GetUserIdString(int index) => $"{ID_BASE}-{index}";
    public static string GetUsernameString(int index) => $"{USERNAME_BASE}-{index}";
    public static string GetEmailString(int index) => $"{EMAIL_BASE}-{index}";

    //-------------------------------------------------------------------------
    public static List<User> GetAllTestUsers() =>
        GetRangeTestUsers(FIRST_INDEX, LAST_INDEX);

    public static List<User> GetRangeTestUsers(int start, int end) =>
        Enumerable.Range(start, end - start + 1).Select(GetTestUser).ToList();

    //-------------------------------------------------------------------------
    public static User GetTestUser(int index) =>
        new()
        {
            Id = $"Id-{index}",
            Email = $"Email-{index}",
            Username = $"Username-{index}",
            FirstName = $"FirstName-{index}",
            LastName = $"LastName-{index}"
        };

    //-------------------------------------------------------------------------
    public static IEnumerable<object?[]> GetUserMemberData()
    {
        var userId = GetUserIdString(EXISTED_INDEX);
        var username = GetUsernameString(EXISTED_INDEX);
        var email = GetEmailString(EXISTED_INDEX);

        // existing users
        yield return new object?[] { userId, null, null };
        yield return new object?[] { null, username, null };
        yield return new object?[] { null, null, email };
        yield return new object?[] { userId, username, email };

        // not existing users
        yield return new object?[] { null, null, null };
        yield return new object?[] { EMPTY_DATA, EMPTY_DATA, EMPTY_DATA };
        yield return new object?[] { NOT_EXISTED_DATA, NOT_EXISTED_DATA, NOT_EXISTED_DATA };
        yield return new object?[] { NOT_EXISTED_DATA, NOT_EXISTED_DATA, email };
        yield return new object?[] { NOT_EXISTED_DATA, username, NOT_EXISTED_DATA };
        yield return new object?[] { NOT_EXISTED_DATA, username, email };
        yield return new object?[] { userId, NOT_EXISTED_DATA, NOT_EXISTED_DATA };
        yield return new object?[] { userId, NOT_EXISTED_DATA, email };
        yield return new object?[] { userId, username, NOT_EXISTED_DATA };
    }

    //-------------------------------------------------------------------------
    public static IEnumerable<object[]> GetUserIdMemberData()
    {
        yield return new object[] { GetUserIdString(EXISTED_INDEX), true };
        yield return new object[] { GetUserIdString(NOT_EXISTED_INDEX), false };
    }

    //-------------------------------------------------------------------------
    public static IEnumerable<object[]> GetUsernameMemberData()
    {
        yield return new object[] { GetUsernameString(EXISTED_INDEX), true };
        yield return new object[] { GetUsernameString(NOT_EXISTED_INDEX), false };
    }

    //-------------------------------------------------------------------------
    public static IEnumerable<object[]> GetEmailMemberData()
    {
        yield return new object[] { GetEmailString(EXISTED_INDEX), true };
        yield return new object[] { GetEmailString(NOT_EXISTED_INDEX), false };
    }

    //-------------------------------------------------------------------------
    public static IEnumerable<object?[]> GetSubstringMemberData()
    {
        yield return new object?[] { null, null, false };
        yield return new object?[] { null, EMPTY_DATA, false };
        yield return new object?[] { EMPTY_DATA, null, false };
        yield return new object?[] { EMPTY_DATA, EMPTY_DATA, false };
        yield return new object?[] { EMPTY_DATA, NOT_EXISTED_DATA, false };
        yield return new object?[] { null, NOT_EXISTED_DATA, false };
        yield return new object?[] { NOT_EXISTED_DATA, NOT_EXISTED_DATA, false };

        yield return new object?[] { USERNAME_BASE, null, true };
        yield return new object?[] { null, EMAIL_BASE, true };
        yield return new object?[] { USERNAME_BASE, EMAIL_BASE, true };
    }

    //-------------------------------------------------------------------------
    public static IEnumerable<object?[]> AddUserMemberData()
    {
        var existedUser = GetTestUser(EXISTED_INDEX);
        var existedUserDto = GetMapper().Map<UserPostDto>(existedUser);

        var newUser = GetTestUser(NOT_EXISTED_INDEX);
        var newUserDto = GetMapper().Map<UserPostDto>(newUser);

        yield return new object?[] { EXISTED_INDEX, existedUserDto };
        yield return new object?[] { NOT_EXISTED_INDEX, newUserDto };
    }

    //-------------------------------------------------------------------------
    public static IEnumerable<object?[]> UpdateUserMemberData()
    {
        var existedUser = GetTestUser(EXISTED_INDEX);
        var existedUserDto = GetMapper().Map<UserPatchDto>(existedUser);
        existedUserDto.Email = UPDATED_EMAIL;

        var notExistedUser = GetTestUser(NOT_EXISTED_INDEX);
        var notExistedUserDto = GetMapper().Map<UserPatchDto>(notExistedUser);

        yield return new object?[] { GetUserIdString(EXISTED_INDEX), existedUserDto };
        yield return new object?[] { GetUserIdString(NOT_EXISTED_INDEX), notExistedUserDto };
    }

    //-------------------------------------------------------------------------
    public static IEnumerable<object?[]> DeleteUserMemberData()
    {
        yield return new object?[] { GetUserIdString(EXISTED_INDEX) };
        yield return new object?[] { GetUserIdString(NOT_EXISTED_INDEX) };
    }

    //-------------------------------------------------------------------------
    public static IMapper GetMapper()
    {
        var configuration = new MapperConfiguration(cfg => { cfg.AddProfile(new MappingProfiles()); });
        return configuration.CreateMapper();
    }

    //-------------------------------------------------------------------------
    public static async Task<UserRepository> GetTestUserRepository()
    {
        return new UserRepository(await GetTestDataContext());
    }

    //-------------------------------------------------------------------------
    private static async Task<DataContext> GetTestDataContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var dataContext = new DataContext(options);
        await dataContext.Database.EnsureCreatedAsync();

        if (await dataContext.Users.AnyAsync())
            return dataContext;

        for (var i = 1; i <= MAX_USER_COUNT; i++)
        {
            dataContext.Add(GetTestUser(i));
            await dataContext.SaveChangesAsync();
        }

        return dataContext;
    }
}
