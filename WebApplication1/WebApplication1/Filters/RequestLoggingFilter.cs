using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Filters;

public class RequestLoggingFilter : IActionFilter
{
    private readonly ILogger<RequestLoggingFilter> _logger;

    public RequestLoggingFilter(ILogger<RequestLoggingFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.HttpContext.Request;
        _logger.LogInformation($"Запрос: {request.Method} {request.Path}");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("Запрос завершён.");
    }
}