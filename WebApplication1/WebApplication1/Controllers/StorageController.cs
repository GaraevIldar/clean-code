using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Filters;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(ExceptionHandlingFilter))] 
[ServiceFilter(typeof(RequestLoggingFilter))] 
public class StorageController : ControllerBase
{
    private readonly MinioService _minioService;

    public StorageController(MinioService minioService)
    {
        _minioService = minioService;
    }

    [HttpPost("upload-text")]
    [ServiceFilter(typeof(AuthorizationFilter))] 
    public async Task<IActionResult> UploadText([FromBody] TextUploadRequest request)
    {
        if (string.IsNullOrEmpty(request.FileName) || string.IsNullOrEmpty(request.Text))
        {
            return BadRequest("FileName and Text are required.");
        }

        var username = HttpContext.Request.Cookies["username"];
        await _minioService.UploadTextAsync(request.FileName, request.Text, username);
        return Ok("Text uploaded successfully.");
    }

    [HttpGet("download-text/{fileName}")]
    [ServiceFilter(typeof(AuthorizationFilter))] 
    public async Task<IActionResult> DownloadText(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("FileName is required.");
        }

        var username = HttpContext.Request.Cookies["username"];
        var text = await _minioService.GetTextAsync(fileName, username);
        return Ok(text);
    }
}



public class TextUploadRequest
{
    public string FileName { get; set; }
    public string Text { get; set; }
}
