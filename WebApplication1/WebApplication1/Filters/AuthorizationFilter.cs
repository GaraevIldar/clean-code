using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication1.Filters;

public class AuthorizationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = context.HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedObjectResult("Неавторизованный доступ.");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}