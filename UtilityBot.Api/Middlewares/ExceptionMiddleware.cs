using System.Net;

namespace UtilityBot.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        HttpStatusCode statusCode;

        switch (exception)
        {
            default:
                statusCode = HttpStatusCode.InternalServerError;
                break;
        }

        _logger.LogError(exception, $"Will add more details when I have custom exceptions later on! But for now.. OMG ERROR: {exception}");

        context.Response.StatusCode = (int)statusCode;
        return context.Response.CompleteAsync();
    }
}