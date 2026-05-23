using Microsoft.AspNetCore.Mvc;
using Platform.Application.Common;

namespace Platform.Api;

public sealed class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var (status, title) = ex switch
            {
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                ForbiddenAccessException => (StatusCodes.Status403Forbidden, "Forbidden"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Not found"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid request"),
                _ => (StatusCodes.Status500InternalServerError, "Server error")
            };

            if (status == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(ex, "Unhandled API exception.");
            }
            else
            {
                logger.LogInformation(ex, "Handled API exception: {Message}", ex.Message);
            }

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = ex.Message,
                Instance = context.Request.Path
            });
        }
    }
}
