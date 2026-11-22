using System.Diagnostics;
using System.Net;

namespace WebApi.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation("Request: {Method} {Path} at {Time}", context.Request.Method, context.Request.Path, DateTime.UtcNow);
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {Method} {Path}", context.Request.Method, context.Request.Path);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "Internal server error." });
        }
        stopwatch.Stop();
        logger.LogInformation("Response: {StatusCode} in {Elapsed} ms", context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}
