using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserDirectory.Api.Constants;
using UserDirectory.Api.Contracts.Common;
using UserDirectory.Api.Exceptions;

namespace UserDirectory.Api.Middleware;

/// <summary>
/// Global exception handling middleware converting domain and system exceptions into standardized API error responses.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items[ApiConstants.CorrelationIdHeader]?.ToString() 
            ?? context.TraceIdentifier;

        var statusCode = HttpStatusCode.InternalServerError;
        var errorCode = ErrorConstants.InternalServerErrorCode;
        var message = ErrorConstants.InternalServerError;
        IDictionary<string, string[]>? errors = null;

        switch (exception)
        {
            case ValidationException valEx:
                statusCode = valEx.StatusCode;
                errorCode = valEx.ErrorCode;
                message = valEx.Message;
                errors = valEx.Errors;
                _logger.LogWarning(valEx, "[{CorrelationId}] Validation error: {Message}", correlationId, valEx.Message);
                break;

            case NotFoundException nfEx:
                statusCode = nfEx.StatusCode;
                errorCode = nfEx.ErrorCode;
                message = nfEx.Message;
                _logger.LogWarning(nfEx, "[{CorrelationId}] Resource not found: {Message}", correlationId, nfEx.Message);
                break;

            case UnauthorizedException unEx:
                statusCode = unEx.StatusCode;
                errorCode = unEx.ErrorCode;
                message = unEx.Message;
                _logger.LogWarning(unEx, "[{CorrelationId}] Unauthorized access attempt: {Message}", correlationId, unEx.Message);
                break;

            case Exceptions.ApplicationException appEx:
                statusCode = appEx.StatusCode;
                errorCode = appEx.ErrorCode;
                message = appEx.Message;
                _logger.LogWarning(appEx, "[{CorrelationId}] Application error: {Message}", correlationId, appEx.Message);
                break;

            default:
                _logger.LogError(exception, "[{CorrelationId}] Unhandled exception: {Message}", correlationId, exception.Message);
                if (_environment.IsDevelopment())
                {
                    message = exception.Message;
                }
                break;
        }

        context.Response.ContentType = ApiConstants.JsonContentType;
        context.Response.StatusCode = (int)statusCode;

        var errorResponse = new ErrorResponse
        {
            Status = (int)statusCode,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors,
            CorrelationId = correlationId,
            Timestamp = DateTime.UtcNow
        };

        var responseJson = JsonSerializer.Serialize(errorResponse, JsonOptions);
        await context.Response.WriteAsync(responseJson);
    }
}

