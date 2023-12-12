using DemoMsUser.Common.Extensions;
using DemoMsUser.Common.Helpers;
using DemoMsUser.Common.Middlewares;
using DemoMsUser.Interfaces;
using DemoMsUser.Repository;
using DemoMsUser.Services;

//-----------------------------------------------------------------------------
var builder = WebApplication.CreateBuilder(args);
builder.AddBaseConfiguration();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//-----------------------------------------------------------------------------
var app = builder.Build();
app.UseMultipleEnvironmentsConfiguration();

app.UseExceptionHandler(ExceptionHandlerHelper.JsonHandler());

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Middlewares
app.UseMiddleware<AccessForbiddenMiddleware>();

app.MapControllers();

app.Run();
