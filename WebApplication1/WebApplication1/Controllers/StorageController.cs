using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StorageController : ControllerBase
{
    private readonly MinioService _minioService;

    public StorageController(MinioService minioService)
    {
        _minioService = minioService;
    }

    [HttpPost("upload-text")]
    public async Task<IActionResult> UploadText([FromBody] TextUploadRequest request)
    {
        if (string.IsNullOrEmpty(request.FileName) || string.IsNullOrEmpty(request.Text))
        {
            return BadRequest("FileName and Text are required.");
        }

        var username = HttpContext.Request.Cookies["username"];
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized("User not authenticated.");
        }

        try
        {
            await _minioService.UploadTextAsync(request.FileName, request.Text, username);
            return Ok("Text uploaded successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpGet("download-text/{fileName}")]
    public async Task<IActionResult> DownloadText(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("FileName is required.");
        }

        var username = HttpContext.Request.Cookies["username"];
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized("User not authenticated.");
        }

        try
        {
            var text = await _minioService.GetTextAsync(fileName, username);
            return Ok(text);
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode(403, "You are not the owner of this document.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }
}



public class TextUploadRequest
{
    public string FileName { get; set; }
    public string Text { get; set; }
}
