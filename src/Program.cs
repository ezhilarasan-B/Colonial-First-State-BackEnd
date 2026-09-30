using System.Diagnostics;
using UserDirectory.Api.Constants;
using UserDirectory.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Services
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// 2. Configure HTTP Request Pipeline
app.UseExceptionMiddleware();
app.UseRequestLoggingMiddleware();

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Colonial First State - Staff Directory API v1");
    c.RoutePrefix = "swagger";
});

// Redirect root / to /swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(ApiConstants.CorsPolicyName);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Automatically open Swagger page when started based on "openswagger" key
var openSwaggerValue = builder.Configuration["openswagger"] ?? builder.Configuration["OpenSwagger"];
var shouldOpenSwagger = string.Equals(openSwaggerValue, "true", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(openSwaggerValue, "1", StringComparison.OrdinalIgnoreCase) ||
                        (openSwaggerValue != null && (openSwaggerValue.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || openSwaggerValue.StartsWith("https://", StringComparison.OrdinalIgnoreCase)));

if (shouldOpenSwagger)
{
    var swaggerUrl = (openSwaggerValue != null && (openSwaggerValue.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || openSwaggerValue.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
        ? openSwaggerValue
        : "http://localhost:5200/swagger/index.html";

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        try
        {
            app.Logger.LogInformation("Automatically opening Swagger documentation at {Url} based on 'openswagger' configuration key.", swaggerUrl);

            if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = swaggerUrl,
                    UseShellExecute = true
                });
            }
            else if (OperatingSystem.IsMacOS())
            {
                Process.Start("open", swaggerUrl);
            }
            else if (OperatingSystem.IsLinux())
            {
                Process.Start("xdg-open", swaggerUrl);
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "Unable to automatically launch browser for Swagger URL: {Url}", swaggerUrl);
        }
    });
}

app.Run();
