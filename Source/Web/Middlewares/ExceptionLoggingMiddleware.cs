using Web.Pages;

namespace Web.Middlewares;

public class ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try 
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Uncaught exception.");
            await Errors.Return500(context);
        }
    }
}