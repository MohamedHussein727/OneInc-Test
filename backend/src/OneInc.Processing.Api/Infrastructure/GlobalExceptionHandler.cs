using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OneInc.Processing.Api.Infrastructure;

/// <summary>
/// Converts unhandled exceptions into safe API responses and structured logs.
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException)
        {
            logger.LogInformation("The request was canceled by the client.");
            return true;
        }

        logger.LogError(exception, "Unexpected failure while processing input.");

        var problem = new ProblemDetails
        {
            Title = "Processing failed",
            Detail = "The server could not complete this request. Please retry.",
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.com/500"
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
