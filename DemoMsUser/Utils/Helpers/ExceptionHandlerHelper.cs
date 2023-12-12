using System.Text;
using DemoMsUser.Common.Constants;
using DemoMsUser.Common.Exceptions;
using DemoMsUser.Common.Responses;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DemoMsUser.Common.Helpers
{
    public static class ExceptionHandlerHelper
    {
        public static Action<IApplicationBuilder> JsonHandler()
        {
            return errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>();
                    if (exception != null)
                    {
                        int statusCode;
                        string statusMessage = string.Empty;
                        switch (exception)
                        {
                            case UserNotExistException:
                                statusCode = StatusCodes.Status404NotFound;
                                statusMessage = StatusMessages.UsersNotExist;
                                break;

                            case UserAlreadyExistsException:
                                statusCode = StatusCodes.Status409Conflict;
                                statusMessage = StatusMessages.UsersAlreadyExist;
                                break;

                            case InvalidInputException:
                                statusCode = StatusCodes.Status400BadRequest;
                                statusMessage = StatusMessages.InvalidInputFields;
                                break;

                            default:
                                statusCode = StatusCodes.Status500InternalServerError;
                                statusMessage = StatusMessages.ServerSomethingWentWrong;
                                break;
                        }

                        var exceptionJson = Encoding.UTF8.GetBytes(
                            JsonConvert.SerializeObject(
                                new ErrorDetails(statusMessage, exception.Error.Message),
                                new JsonSerializerSettings
                                {
                                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                                })
                        );

                        context.Response.StatusCode = statusCode;
                        context.Response.ContentType = "application/json";
                        await context.Response.Body.WriteAsync(exceptionJson, 0, exceptionJson.Length);
                    }
                });
            };
        }
    }
}
