using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class UserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> RegisterUserAsync(string userName, string password)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        if (existingUser != null)
        {
            _logger.LogWarning("Пользователь с таким именем уже существует.");
            return false;
        }
        
        if (!ValidatePassword(password))
        {
            _logger.LogWarning("Пароль не удовлетворяет требованиям.");
            return false; 
        }
        
        string salt = GenerateSalt();
        string hashedPassword = HashPassword(password, salt);
        
        var user = new User
        {
            UserName = userName,
            UserPassword = hashedPassword,
            UserSalt = salt
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<User> AuthenticateUserAsync(string userName, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        if (user == null)
        {
            return null;
        }
        
        string hashedPassword = HashPassword(password, user.UserSalt);
        if (user.UserPassword == hashedPassword)
        {
            return user;
        }

        return null;
    }
    private bool ValidatePassword(string password)
    {
        if (password.Length < 8)
            return false;
        
        if (!password.Any(char.IsDigit))
            return false;
        
        if (!password.Any(char.IsUpper))
            return false;
        
        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            return false;

        return true;
    }
    
    private string HashPassword(string password, string salt)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var saltedPassword = password + salt;
            var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hash);
        }
    }
    
    private string GenerateSalt()
    {
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            var saltBytes = new byte[32]; 
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }
    }
}
