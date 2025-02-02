using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using WebApplication1.Filters;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(ExceptionHandlingFilter))] 
[ServiceFilter(typeof(RequestLoggingFilter))] 
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Логин и пароль обязательны.");

        var success = await _userService.RegisterUserAsync(request.UserName, request.Password);
        if (!success)
            return BadRequest("Пользователь с таким именем уже существует или пароль не удовлетворяет требованиям.");

        return Ok("Регистрация успешна.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Логин и пароль обязательны.");

        var user = await _userService.AuthenticateUserAsync(request.UserName, request.Password);
        if (user == null)
            return Unauthorized("Неверный логин или пароль.");

        HttpContext.Session.SetString("UserId", user.UserId.ToString());
        HttpContext.Session.SetString("UserName", user.UserName);

        Response.Cookies.Append("UserName", user.UserName, new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTimeOffset.Now.AddMinutes(30)
        });

        return Ok(new { Username = user.UserName });
    }

    [HttpPost("logout")]
    [ServiceFilter(typeof(AuthorizationFilter))] 
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        Response.Cookies.Delete("UserName");

        return Ok("Выход выполнен успешно.");
    }

    [HttpGet("check-auth")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> CheckAuth()
    {
        var userId = HttpContext.Session.GetString("UserId");
        var userName = HttpContext.Session.GetString("UserName");

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
        {
            return Unauthorized("Пользователь не авторизован.");
        }

        return Ok(new { UserId = userId, UserName = userName });
    }

}

public class RegisterRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class LoginRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
}


