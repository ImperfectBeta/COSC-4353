using System.Security.Cryptography;
using System.Text;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services;

public interface IAuthService
{
    (AuthResponse? response, string? error) Register(RegisterRequest request);
    (AuthResponse? response, string? error) Login(LoginRequest request);
}

public class AuthService : IAuthService
{
    private readonly IUserStore _userStore;
    private static readonly Dictionary<string, int> _sessions = new();

    public AuthService(IUserStore userStore)
    {
        _userStore = userStore;
    }

    public (AuthResponse? response, string? error) Register(RegisterRequest request)
    {
        // required fields
        if (string.IsNullOrWhiteSpace(request.Name))
            return (null, "Name is required.");

        if (request.Name.Length > 100)
            return (null, "Name must be 100 characters or fewer.");

        if (string.IsNullOrWhiteSpace(request.Email))
            return (null, "Email is required.");

        if (request.Email.Length > 200)
            return (null, "Email must be 200 characters or fewer.");

        if (!IsValidEmail(request.Email))
            return (null, "Email format is invalid.");

        if (string.IsNullOrWhiteSpace(request.Password))
            return (null, "Password is required.");

        if (request.Password.Length < 8)
            return (null, "Password must be at least 8 characters.");

        if (request.Password.Length > 200)
            return (null, "Password must be 200 characters or fewer.");

        // check for duplicate email
        if (_userStore.EmailExists(request.Email))
            return (null, "An account with this email already exists.");

        // parse role, default to User if unrecognised
        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
            role = UserRole.User;

        // create and store user
        var user = new User
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = HashPassword(request.Password),
            Role = role
        };

        var created = _userStore.Add(user);
        var token = GenerateToken(created.Id);

        return (BuildResponse(created, token), null);
    }

    public (AuthResponse? response, string? error) Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return (null, "Email is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            return (null, "Password is required.");

        var user = _userStore.GetByEmail(request.Email.Trim());

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            return (null, "Invalid email or password.");

        var token = GenerateToken(user.Id);
        return (BuildResponse(user, token), null);
    }

    // password hashing
    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    public static bool VerifyPassword(string password, string hash) =>
        HashPassword(password) == hash;

    // generates a random session token
    private static string GenerateToken(int userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        _sessions[token] = userId;
        return token;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email.Trim();
        }
        catch
        {
            return false;
        }
    }

    private static AuthResponse BuildResponse(User user, string token) => new()
    {
        UserId = user.Id,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role.ToString(),
        Token = token
    };

    public static bool TryGetUserId(string token, out int userId) =>
        _sessions.TryGetValue(token, out userId);
}