using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using WebApplication1.Model;

public class MinioService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MinioService(IOptions<MinioSettings> minioSettings, ApplicationDbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        var settings = minioSettings.Value;
        _minioClient = new MinioClient()
            .WithEndpoint(settings.Endpoint)
            .WithCredentials(settings.AccessKey, settings.SecretKey)
            .Build();

        _bucketName = settings.BucketName;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> GrantAccessToFile(UserFileAccessRequest request)
    {
        if (string.IsNullOrEmpty(request.UserName) || request.DocumentId == 0 || string.IsNullOrEmpty(request.Role))
        {
            throw new ArgumentException("All fields are required.");
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var currentUserName = _httpContextAccessor.HttpContext?.Request.Cookies["username"];
        if (string.IsNullOrEmpty(currentUserName))
        {
            throw new Exception("User not authenticated.");
        }

        var currentUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == currentUserName);
        if (currentUser == null)
        {
            throw new Exception("Current user not found.");
        }
        
        var documentAccess = await _dbContext.DocumentAccesses
            .FirstOrDefaultAsync(da => da.DocumentId == request.DocumentId && da.UserId == currentUser.UserId);

        if (documentAccess == null || documentAccess.Status != "creator")
        {
            throw new UnauthorizedAccessException("Only the creator can grant access.");
        }

        var existingAccess = await _dbContext.DocumentAccesses
            .FirstOrDefaultAsync(da => da.DocumentId == request.DocumentId && da.UserId == user.UserId);

        if (existingAccess != null)
        {
            throw new UnauthorizedAccessException("User already has access to this document.");
        }

        var newAccess = new DocumentAccess
        {
            DocumentId = request.DocumentId,
            UserId = user.UserId,
            Status = request.Role
        };

        _dbContext.DocumentAccesses.Add(newAccess);
        await _dbContext.SaveChangesAsync();

        return true;
    }


    public async Task<bool> UploadTextAsync(string fileName, string text, string username)
    {
        await EnsureBucketExistsAsync();

        if (string.IsNullOrEmpty(username))
        {
            username = _httpContextAccessor.HttpContext?.Request.Cookies["username"];
        }

        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("User not authenticated.");
        }

        var fileWithUser = $"{fileName}";

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null)
        {
            throw new Exception("User not found.");
        }
        
        var existingDocument = await _dbContext.Documents
            .Include(d => d.DocumentAccesses)
            .FirstOrDefaultAsync(d => d.DocumentName == fileWithUser);

        if (existingDocument != null)
        {
            var userHasAccess = existingDocument.DocumentAccesses
                .Any(da => da.UserId == user.UserId && (da.Status == "creator" || da.Status == "editor"));

            if (!userHasAccess)
            {
                throw new UnauthorizedAccessException("A file with this name already exists for another user.");
            }
        }
        
        if (existingDocument == null)
        {
            var newDocument = new Document
            {
                DocumentName = fileWithUser
            };

            await _dbContext.AddAsync(newDocument);
            await _dbContext.SaveChangesAsync();
            
            var documentAccess = new DocumentAccess
            {
                UserId = user.UserId,
                DocumentId = newDocument.DocumentId,
                Status = "creator"
            };

            await _dbContext.AddAsync(documentAccess);
            await _dbContext.SaveChangesAsync();
        }
        
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
        {
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileWithUser)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length));
        }

        return true;
    }


    public async Task<string> GetTextAsync(string fileName, string username = null)
    {
        if (string.IsNullOrEmpty(username))
        {
            username = _httpContextAccessor.HttpContext?.Request.Cookies["username"];
        }

        if (string.IsNullOrEmpty(username))
        {
            throw new Exception("User not authenticated.");
        }

        var fileWithUser = $"{fileName}";

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null)
        {
            throw new Exception("User not found.");
        }
        
        var document = await _dbContext.Documents
            .Include(d => d.UserDocuments)
            .FirstOrDefaultAsync(d => d.DocumentName == fileWithUser);

        if (document == null)
        {
            throw new Exception("Document not found.");
        }
        
        var documentAccess = await _dbContext.DocumentAccesses
            .FirstOrDefaultAsync(da => da.UserId == user.UserId && da.DocumentId == document.DocumentId);

        if (documentAccess == null)
        {
            throw new UnauthorizedAccessException("User does not have access to this document.");
        }
        
        if (documentAccess.Status != "creator" && documentAccess.Status != "editor" &&
            documentAccess.Status != "reader")
        {
            throw new UnauthorizedAccessException("User does not have permission to read this document.");
        }
        
        var stream = new MemoryStream();
        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileWithUser)
            .WithCallbackStream(sourceStream => { sourceStream.CopyTo(stream); }));

        stream.Position = 0;
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            return await reader.ReadToEndAsync();
        }
    }


    private async Task EnsureBucketExistsAsync()
    {
        bool exists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
        if (!exists)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
        }
    }
}