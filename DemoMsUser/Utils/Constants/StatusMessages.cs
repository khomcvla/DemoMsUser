namespace DemoMsUser.Common.Constants
{
    public static class StatusMessages
    {
        public const string AccessForbidden = "Access to this information is restricted due to security constraints for the user!";

        public const string AlreadyExist = "This unique data already exists in the system.";

        public const string InvalidSubField = "The access token does not provide the 'sub' field.";
        public const string InvalidInputFields = "An error occurred due to invalid input fields. Please check the entered fields and ensure they meet the required criteria.";

        public const string ServerSomethingWentWrong = "Something went wrong on the server. Please try again later.";

        public const string ServerUsersAddFailed = $"Cannot create user(s)! {ServerSomethingWentWrong}";
        public const string ServerUsersDeleteFailed = $"Cannot delete user(s)! {ServerSomethingWentWrong}";
        public const string ServerUsersGetFailed = $"Cannot get user(s)! {ServerSomethingWentWrong}";
        public const string ServerUsersUpdateFailed = $"Cannot update user(s)! {ServerSomethingWentWrong}";
        public const string ServerUsersValidationFailed = $"Cannot validate user(s)! {ServerSomethingWentWrong}";

        public const string StatusSuccess = "Success";

        public const string UsersAlreadyExist = "One or more users already exist! Please check the details of the user(s) and try again.";
        public const string UsersNotExist = "One or more users do not exist! Please check the details of the user(s) and try again.";

        public const string UsersAddFailed = "User(s) addition failed! Please check the details of the user(s) and try again.";
        public const string UsersDeleteFailed = "User(s) deletion failed! Please check the details of the user(s) and try again.";
        public const string UsersGetFailed = "User(s) retrieval failed! Please check the details of the user(s) and try again.";
        public const string UsersUpdateFailed = "User(s) update failed! Please check the details of the user(s) and try again.";

        public const string ValidationFailed = "Validation Failed";
        public const string ValidationSuccess = "Validation Successful";
    }
}
