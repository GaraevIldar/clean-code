using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

[ApiController]
[Route("api/[controller]")]
public class TextController : ControllerBase
{
    private readonly HelperService _helperService;

    public TextController(HelperService helperService)
    {
        _helperService = helperService;
    }
    [HttpPost("process")]
    public IActionResult ProcessText([FromBody] TextRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest("Текст не должен быть пустым.");
        }

        var result = _helperService.ProcessText(request.Text);
        return Ok(new { Output = result });
    }
}

public class TextRequest
{
    public string Text { get; set; }
}