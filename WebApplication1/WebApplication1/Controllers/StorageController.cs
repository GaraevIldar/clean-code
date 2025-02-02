using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Filters;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(ExceptionHandlingFilter))]
[ServiceFilter(typeof(RequestLoggingFilter))]
public class StorageController : ControllerBase
{
    private readonly MinioService _minioService;
    private readonly ApplicationDbContext _dbContext;

    public StorageController(MinioService minioService, ApplicationDbContext dbContext)
    {
        _minioService = minioService;
        _dbContext = dbContext;
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

    [HttpGet("get-document-id")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> GetDocumentIdByFileName([FromQuery] string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("FileName is required.");
        }

        var document = await _dbContext.Documents
            .FirstOrDefaultAsync(d => d.DocumentName == fileName);

        if (document == null)
        {
            return NotFound("Document not found.");
        }

        return Ok(new { documentId = document.DocumentId });
    }
    [HttpPost("grant-access")]
    [ServiceFilter(typeof(AuthorizationFilter))]
    public async Task<IActionResult> GrantAccessToFile([FromBody] UserFileAccessRequest request)
    {
        try
        {
            await _minioService.GrantAccessToFile(request);
            return Ok("Access granted successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class UserFileAccessRequest
{
    public string UserName { get; set; }
    public int DocumentId { get; set; } 
    public string Role { get; set; } 
}

public class TextUploadRequest
{
    public string FileName { get; set; }
    public string Text { get; set; }
}