using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using UserDirectory.Api.Infrastructure.Authentication;

namespace UserDirectory.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = new JwtOptions();
        configuration.GetSection(JwtOptions.SectionName).Bind(jwtOptions);

        if (string.IsNullOrWhiteSpace(jwtOptions.Secret))
        {
            throw new InvalidOperationException("JWT Secret is not configured.");
        }

        var key = Encoding.UTF8.GetBytes(jwtOptions.Secret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = !string.IsNullOrWhiteSpace(jwtOptions.Issuer),
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = !string.IsNullOrWhiteSpace(jwtOptions.Audience),
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("StaffRead", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("permission", "staff:read") ||
                    ctx.User.IsInRole("Admin") ||
                    ctx.User.HasClaim("role", "Admin") ||
                    ctx.User.HasClaim("role", "StaffReadOnly") ||
                    ctx.User.HasClaim("role", "ReadOnly") ||
                    ctx.User.IsInRole("StaffReadOnly") ||
                    ctx.User.IsInRole("ReadOnly")));

            options.AddPolicy("StaffWrite", policy =>
                policy.RequireAssertion(ctx =>
                    (ctx.User.HasClaim("permission", "staff:write") ||
                     ctx.User.IsInRole("Admin") ||
                     ctx.User.HasClaim("role", "Admin")) &&
                    !ctx.User.IsInRole("StaffReadOnly") &&
                    !ctx.User.HasClaim("role", "StaffReadOnly") &&
                    !ctx.User.IsInRole("ReadOnly") &&
                    !ctx.User.HasClaim("role", "ReadOnly")));

            options.AddPolicy("ClientRead", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim("permission", "client:read") ||
                    ctx.User.IsInRole("Admin") ||
                    ctx.User.HasClaim("role", "Admin") ||
                    ctx.User.HasClaim("role", "StaffReadOnly") ||
                    ctx.User.HasClaim("role", "ReadOnly") ||
                    ctx.User.IsInRole("StaffReadOnly") ||
                    ctx.User.IsInRole("ReadOnly")));

            options.AddPolicy("ClientWrite", policy =>
                policy.RequireAssertion(ctx =>
                    (ctx.User.HasClaim("permission", "client:write") ||
                     ctx.User.IsInRole("Admin") ||
                     ctx.User.HasClaim("role", "Admin")) &&
                    !ctx.User.IsInRole("StaffReadOnly") &&
                    !ctx.User.HasClaim("role", "StaffReadOnly") &&
                    !ctx.User.IsInRole("ReadOnly") &&
                    !ctx.User.HasClaim("role", "ReadOnly")));
        });
        return services;
    }
}
