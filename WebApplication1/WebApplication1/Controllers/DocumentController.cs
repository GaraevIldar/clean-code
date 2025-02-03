using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

public class DocumentController : Controller
{
    private readonly DocumentService _documentService;
    private readonly ApplicationDbContext _context;

    public DocumentController(DocumentService documentService, ApplicationDbContext dbContext)
    {
        _documentService = documentService;
        _context = dbContext;
    }
    
    [HttpGet("documents/by-username/{userName}")]
    public async Task<IActionResult> GetDocumentsByUserName(string userName)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }
            
            var documents = await _documentService.GetUserDocumentsWithCreatorOrEditorStatusAsync(user.UserId);
        
            return Ok(documents);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Произошла ошибка при получении документов.");
        }
    }

}
