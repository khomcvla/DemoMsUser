using DemoMsUser.Common.Constants;
using Newtonsoft.Json;

namespace DemoMsUser.Common.Middlewares
{
    public class AccessForbiddenMiddleware
    {
        private readonly RequestDelegate _next;
        public AccessForbiddenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                await context.Response.WriteAsync(
                    JsonConvert.SerializeObject(
                        StatusMessages.AccessForbidden));
            }
        }
    }
}
