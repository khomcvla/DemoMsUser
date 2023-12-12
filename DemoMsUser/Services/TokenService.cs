using DemoMsUser.Common.Constants;
using DemoMsUser.Interfaces;

namespace DemoMsUser.Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsTokenValid()
        {
            // check if the access token contains all required fields
            var role = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(c => c.Type.Equals("role"))?.Value;

            var sub = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(c => c.Type.Equals("sub"))?.Value;

            return (role is not null) && (sub is not null);
        }

        public bool IsAdmin()
        {
            var role = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(c => c.Type.Equals("role"))?.Value;

            return (role is not null) && role.Equals(UserRoles.Admin);
        }

        public string? GetSubFromToken()
        {
            return _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(c => c.Type.Equals("sub"))?.Value;
        }
    }
}
