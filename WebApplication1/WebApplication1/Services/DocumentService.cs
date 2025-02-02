using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Services;

public class DocumentService
{
    private readonly ApplicationDbContext _context;

    public DocumentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Document>> GetUserDocumentsWithCreatorOrEditorStatusAsync(int userId)
    {
        var documents = await _context.DocumentAccesses
            .Where(da => da.UserId == userId && (da.Status == "creator" || da.Status == "editor"))
            .Select(da => da.Document)
            .ToListAsync();

        return documents;
    }
}