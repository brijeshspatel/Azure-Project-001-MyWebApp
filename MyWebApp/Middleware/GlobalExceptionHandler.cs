using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Core.Exceptions;

namespace Azure_Project_001_MyWebApp.Middleware;

/// <summary>
/// Global exception handler that converts domain exceptions to appropriate HTTP responses.
/// Implements W3C Trace Context standard for distributed tracing with proper correlation IDs.
/// </summary>
/// <remarks>
/// This handler follows Microsoft's best practices for distributed tracing:
/// - Uses Activity.Current to access W3C Trace Context (traceId, spanId, parentSpanId)
/// - TraceId: 16-byte (32-character hex) globally unique identifier for the entire distributed trace
/// - SpanId: 8-byte (16-character hex) unique identifier for this specific operation
/// - Supports correlation across microservices and distributed systems
/// - Compatible with OpenTelemetry, Azure Monitor/Application Insights, and other APM tools
/// </remarks>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    /// Initialises a new instance of the <see cref="GlobalExceptionHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        // Get the current Activity for W3C Trace Context
        // Activity.Current is automatically set by ASP.NET Core and contains W3C TraceContext IDs
        var activity = Activity.Current;
        
        // Get W3C TraceId (or fallback to HttpContext.TraceIdentifier for backwards compatibility)
        // TraceId is the globally unique identifier that stays the same across all services in a distributed trace
        var traceId = activity?.TraceId.ToHexString() ?? httpContext.TraceIdentifier;
        
        // Get W3C SpanId - unique identifier for this specific operation/span
        var spanId = activity?.SpanId.ToHexString();

        _logger.LogError(
            exception,
            "An exception occurred: {ExceptionType} - {Message} | TraceId: {TraceId} | SpanId: {SpanId}",
            exception.GetType().Name,
            exception.Message,
            traceId,
            spanId);

        ProblemDetails problemDetails;

        switch (exception)
        {
            case ValidationException validationException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "One or more validation errors occurred.",
                    Detail = validationException.Message,
                    Instance = httpContext.Request.Path
                };

                // Add validation errors to extensions
                if (validationException.Errors.Any())
                {
                    problemDetails.Extensions["errors"] = validationException.Errors;
                }

                if (!string.IsNullOrEmpty(validationException.ErrorCode))
                {
                    problemDetails.Extensions["errorCode"] = validationException.ErrorCode;
                }

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            case NotFoundException notFoundException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                    Title = "Resource not found.",
                    Detail = notFoundException.Message,
                    Instance = httpContext.Request.Path
                };

                if (!string.IsNullOrEmpty(notFoundException.ErrorCode))
                {
                    problemDetails.Extensions["errorCode"] = notFoundException.ErrorCode;
                }

                problemDetails.Extensions["entityType"] = notFoundException.EntityType;
                problemDetails.Extensions["entityId"] = notFoundException.EntityId?.ToString() ?? string.Empty;

                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                break;

            case WeatherForecastException weatherException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "Weather forecast error.",
                    Detail = weatherException.Message,
                    Instance = httpContext.Request.Path
                };

                if (!string.IsNullOrEmpty(weatherException.ErrorCode))
                {
                    problemDetails.Extensions["errorCode"] = weatherException.ErrorCode;
                }

                if (weatherException.RequestedDays.HasValue)
                {
                    problemDetails.Extensions["requestedDays"] = weatherException.RequestedDays.Value;
                }

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            case DomainException domainException:
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "A domain error occurred.",
                    Detail = domainException.Message,
                    Instance = httpContext.Request.Path
                };

                if (!string.IsNullOrEmpty(domainException.ErrorCode))
                {
                    problemDetails.Extensions["errorCode"] = domainException.ErrorCode;
                }

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            default:
                // For unexpected exceptions, return a generic error
                problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                    Title = "An error occurred while processing your request.",
                    Detail = "An unexpected error occurred. Please try again later.",
                    Instance = httpContext.Request.Path
                };

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        // Add W3C Trace Context correlation IDs for distributed tracing
        // These IDs enable end-to-end tracing across microservices and distributed systems
        
        // TraceId: Globally unique identifier (16-byte/32-character hex) for the entire distributed trace
        // This ID remains the same across all services involved in processing a single request
        problemDetails.Extensions["traceId"] = traceId;

        // SpanId: Unique identifier (8-byte/16-character hex) for this specific operation within the trace
        // Each service/operation has its own SpanId, enabling drill-down to specific components
        if (!string.IsNullOrEmpty(spanId))
        {
            problemDetails.Extensions["spanId"] = spanId;
        }

        // ParentSpanId: Identifier of the parent operation that called this service
        // Enables building the complete trace tree for distributed requests
        if (activity?.ParentSpanId != null)
        {
            problemDetails.Extensions["parentSpanId"] = activity.ParentSpanId.ToHexString();
        }

        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Exception has been handled
    }
}
