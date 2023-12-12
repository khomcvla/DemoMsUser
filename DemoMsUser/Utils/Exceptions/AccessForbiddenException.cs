namespace DemoMsUser.Common.Exceptions;

public class AccessForbiddenException : Exception
{
    public AccessForbiddenException(List<string> messages) : base(String.Join("; ", messages))
    {
    }

    public AccessForbiddenException(string message) : base(message)
    {
    }

    public AccessForbiddenException(string message, Exception inner) : base(message, inner)
    {
    }
}
