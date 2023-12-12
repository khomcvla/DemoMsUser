namespace DemoMsUser.Interfaces
{
    public interface ITokenService
    {
        bool IsTokenValid();
        bool IsAdmin();
        string? GetSubFromToken();
    }
}
