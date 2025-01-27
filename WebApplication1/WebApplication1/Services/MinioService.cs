using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        var fileWithUser = $"{fileName}_{username}";

        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.UserName == username);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var existingUserDocument = await _dbContext.UserDocuments
            .Where(ud => ud.UserId == user.UserId)
            .Join(_dbContext.Documents, ud => ud.DocumentId, d => d.DocumentId, (ud, d) => new { ud, d })
            .Where(x => x.d.DocumentName == fileWithUser)
            .FirstOrDefaultAsync();

        if (existingUserDocument != null)
        {
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

        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
        {
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileWithUser)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length));
        }

        var document = new Document
        {
            DocumentName = fileWithUser
        };

        await _dbContext.AddAsync(document);
        await _dbContext.SaveChangesAsync();

        var userDocument = new UserDocument
        {
            UserId = user.UserId,
            DocumentId = document.DocumentId
        };

        await _dbContext.AddAsync(userDocument);
        await _dbContext.SaveChangesAsync();

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

        var fileWithUser = $"{fileName}_{username}";

        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.UserName == username);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var document = await _dbContext.Documents
            .Include(d => d.UserDocuments)
            .SingleOrDefaultAsync(d => d.DocumentName == fileWithUser);

        if (document == null)
        {
            throw new Exception("Document not found.");
        }

        if (!document.UserDocuments.Any(ud => ud.UserId == user.UserId))
        {
            throw new UnauthorizedAccessException("User is not the owner of this document.");
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