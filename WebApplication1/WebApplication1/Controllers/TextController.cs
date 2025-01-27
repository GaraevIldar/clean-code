using Microsoft.AspNetCore.Mvc;
using WebApplication1.Filters;
using WebApplication1.Services;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(ExceptionHandlingFilter))] 
[ServiceFilter(typeof(RequestLoggingFilter))] 
public class TextController : ControllerBase
{
    private readonly TextService _textService;

    public TextController(TextService textService)
    {
        _textService = textService;
    }
    [HttpPost("process")]
    public IActionResult ProcessText([FromBody] TextRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest("Текст не должен быть пустым.");
        }

        var result = _textService.ProcessText(request.Text);
        return Ok(new { Output = result });
    }
}

public class TextRequest
{
    public string Text { get; set; }
}