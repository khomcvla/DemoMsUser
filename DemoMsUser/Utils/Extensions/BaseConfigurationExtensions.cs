﻿using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using IdentityServer4.AccessTokenValidation;
using Microsoft.EntityFrameworkCore;
using DemoMsUser.Common.Constants;
using DemoMsUser.Common.Handlers;
using DemoMsUser.Data;
using Microsoft.IdentityModel.Tokens;

namespace DemoMsUser.Common.Extensions;

public static class BaseConfigurationExtensions
{
    public static WebApplicationBuilder AddBaseConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors();
        builder.Services.AddControllersWithJsonConfiguration();

        builder.Services.AddAzureKeyVault();

        builder.AddLoggingConfiguration();

        builder.AddDataContextConfiguration();

        builder.AddAuthorizationConfiguration();
        builder.AddAuthenticationConfiguration();

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env == Environments.Development)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        return builder;
    }

    private static IServiceCollection AddAzureKeyVault(this IServiceCollection services)
    {
        var valueSecretIssuer = "TEST_SECRET_ISSUER";
        var valueSecretAudience = "TEST_SECRET_AUDIENCE";
        var valueSecretSigningKey = "TEST_SECRET_SIGNING_KEY";

        var secretClient = new SecretClient(new Uri("https://{link-to-akv}.vault.azure.net/"), new DefaultAzureCredential());
        var keyclient = new KeyClient(new Uri("https://{link-to-akv} .vault.azure.net/"), new DefaultAzureCredential());

        KeyVaultSecret resultSecretIssuer = secretClient.GetSecret ("secret-issuer");
        KeyVaultSecret resultSecretAudience = secretClient.GetSecret("secret-audience");
        KeyVaultSecret resultSecretSigningKey = secretClient.GetSecret ("issuer-signing-key");

        valueSecretIssuer = resultSecretIssuer.Value;
        valueSecretAudience = resultSecretAudience.Value;
        valueSecretSigningKey = resultSecretSigningKey.Value;

        if (string.IsNullOrEmpty(valueSecretIssuer) || string.IsNullOrEmpty(valueSecretAudience) ||
            string.IsNullOrEmpty(valueSecretSigningKey))
            throw new Exception (StatusMessages.AccessSecrets);

        return services;
    }

    private static IServiceCollection AddControllersWithJsonConfiguration(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

        return services;
    }

    private static WebApplicationBuilder AddLoggingConfiguration(this WebApplicationBuilder builder)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        builder.Services.AddLogging(loggingBuilder =>
        {
            if (env == Environments.Production)
            {
                loggingBuilder.Services.AddApplicationInsightsTelemetry(options =>
                {
                    options.ConnectionString = builder.Configuration.GetValue<string>("ApplicationInsights:ConnectionString");
                });
                loggingBuilder.AddApplicationInsights();
            }
            else
            {
                loggingBuilder.AddConsole();
            }
        });

        return builder;
    }

    private static WebApplicationBuilder AddDataContextConfiguration(this WebApplicationBuilder builder)
    {
        var assembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var connectionString = builder.Configuration.GetConnectionString(env);

        builder.Services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(assembly));
        });

        return builder;
    }

    private static WebApplicationBuilder AddAuthenticationConfiguration(this WebApplicationBuilder builder)
    {
        var assemblyName = typeof(Program).GetTypeInfo().Assembly.GetName().Name?.ToLower();

        // Rename claims (ex. "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" -> "role")
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        // Azure Key Vault
        var valueSecretIssuer = "TEST_SECRET_ISSUER";
        var valueSecretAudience = "TEST_SECRET_AUDIENCE";
        var valueSecretSigningKey = "TEST_SECRET_SIGNING_KEY";

        var secretClient = new SecretClient(new Uri("https://{link-to-akv}.vault.azure.net/"), new DefaultAzureCredential());
        var keyclient = new KeyClient(new Uri("https://{link-to-akv} .vault.azure.net/"), new DefaultAzureCredential());

        KeyVaultSecret resultSecretIssuer = secretClient.GetSecret ("secret-issuer");
        KeyVaultSecret resultSecretAudience = secretClient.GetSecret("secret-audience");
        KeyVaultSecret resultSecretSigningKey = secretClient.GetSecret ("issuer-signing-key");

        valueSecretIssuer = resultSecretIssuer.Value;
        valueSecretAudience = resultSecretAudience.Value;
        valueSecretSigningKey = resultSecretSigningKey.Value;

        if (string.IsNullOrEmpty(valueSecretIssuer) || string.IsNullOrEmpty(valueSecretAudience) ||
            string.IsNullOrEmpty(valueSecretSigningKey))
            throw new Exception (StatusMessages.AccessSecrets);

        builder.Services.AddAuthentication(options =>
            {
                //NOTE: see https://github.com/IdentityServer/IdentityServer4/issues/1525
                options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
            })
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = "https://localhost:7286/";
                options.ApiName = assemblyName;
                options.RequireHttpsMetadata = false;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(valueSecretSigningKey)),
                    ValidateIssuer = true,
                    ValidIssuer = valueSecretIssuer,
                    ValidateAudience = true,
                    ValidAudience = valueSecretAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return builder;
    }

    private static WebApplicationBuilder AddAuthorizationConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            // ADMIN
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireClaim("role", UserRoles.Admin));

            // ID from route and ID from access token is similar
            options.AddPolicy("SubIdMatch", policy =>
                policy.AddRequirements(new SubIdMatchRequirement()));

            // is ADMIN or SUB and ID are same
            options.AddPolicy("AdminOnly, SubIdMatch", policy =>
                policy.AddRequirements(new AdminOnlyOrSubIdMatchRequirement()));
        });

        return builder;
    }

    public static WebApplication UseMultipleEnvironmentsConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (app.Environment.IsProduction())
        {
            app.UseHsts();
        }

        return app;
    }
}
