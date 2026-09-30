using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserDirectory.Api.Application.Behaviors;
using UserDirectory.Api.Constants;
using UserDirectory.Api.Data.DatabaseConnection;
using UserDirectory.Api.Infrastructure.Authentication;
using UserDirectory.Api.Repositories;
using UserDirectory.Api.Contracts.Interfaces.Repositories;
using UserDirectory.Api.Services;
using UserDirectory.Api.Contracts.Interfaces.Services;
using UserDirectory.Api.Validators;

namespace UserDirectory.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // FluentValidation - scans and registers all validators from this assembly
        services.AddValidatorsFromAssemblyContaining<CreateStaffRequestValidator>();

        // MediatR with Validation Pipeline Behavior
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Database Factory
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        // Repositories
        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAuthUserRepository, AuthUserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Services
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthUserService, AuthUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // CORS
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ??
            new[] { "http://localhost:5173", "http://localhost:3000" };

        services.AddCors(options =>
        {
            options.AddPolicy(ApiConstants.CorsPolicyName, policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .WithExposedHeaders(ApiConstants.CorrelationIdHeader);
            });
        });

        return services;
    }
}
