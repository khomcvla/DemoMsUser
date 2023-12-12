namespace DemoMsUser.Common.Exceptions;

public class InvalidInputException : Exception
{
    public InvalidInputException()
    {
    }

    public InvalidInputException(List<string> messages) : base(String.Join("; ", messages))
    {
    }

    public InvalidInputException(string message) : base(message)
    {
    }

    public InvalidInputException(string message, Exception inner) : base(message, inner)
    {
    }
}
