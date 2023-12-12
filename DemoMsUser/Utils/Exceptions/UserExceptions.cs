namespace DemoMsUser.Common.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(List<string> messages) : base(String.Join("; ", messages))
        {
        }

        public UserAlreadyExistsException(string message) : base(message)
        {
        }

        public UserAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class UserNotExistException : Exception
    {
        public UserNotExistException()
        {
        }

        public UserNotExistException(List<string> messages) : base(String.Join("; ", messages))
        {
        }

        public UserNotExistException(string message) : base(message)
        {
        }

        public UserNotExistException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
