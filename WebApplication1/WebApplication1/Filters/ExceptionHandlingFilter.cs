using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Filters;

public class ExceptionHandlingFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionHandlingFilter> _logger;

    public ExceptionHandlingFilter(ILogger<ExceptionHandlingFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Ошибка в процессе обработки запроса.");
        context.Result = new ObjectResult("Произошла ошибка сервера.")
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = true;
    }
}