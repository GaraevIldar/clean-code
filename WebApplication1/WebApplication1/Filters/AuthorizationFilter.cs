using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication1.Filters;

public class AuthorizationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        try
        {
            var userId = context.HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedObjectResult("Неавторизованный доступ.");
            }
        }
        catch (Exception ex)
        {
            context.Result = new StatusCodeResult(500);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
