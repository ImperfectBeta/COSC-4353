using Microsoft.EntityFrameworkCore;
using QueueSmart.Api.Data;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services;

public interface IUserStore
{
    User? GetByEmail(string email);
    User? GetById(int id);
    IEnumerable<User> GetAll();
    User Add(User user);
    void UpdateUser(User user);
    bool EmailExists(string email);
}

public class DbUserStore : IUserStore
{
    private readonly AppDbContext _db;

    public DbUserStore(AppDbContext db)
    {
        _db = db;
    }

    public User? GetByEmail(string email) =>
        _db.UserCredentials
           .Include(c => c.Profile)
           .FirstOrDefault(c => c.Email == email.ToLowerInvariant())
           .ToUser();

    public User? GetById(int id) =>
        _db.UserCredentials
           .Include(c => c.Profile)
           .FirstOrDefault(c => c.Id == id)
           .ToUser();

    public IEnumerable<User> GetAll() =>
        _db.UserCredentials
           .Include(c => c.Profile)
           .AsEnumerable()
           .Select(c => c.ToUser()!);

    public User Add(User user)
    {
        var credential = new UserCredential
        {
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            Role = user.Role,
            Profile = new UserProfile { FullName = user.Name }
        };

        _db.UserCredentials.Add(credential);
        _db.SaveChanges();

        user.Id = credential.Id;
        return user;
    }

    public bool EmailExists(string email) =>
        _db.UserCredentials.Any(c => c.Email == email.ToLowerInvariant());

    public void UpdateUser(User user)
    {
        var credential = _db.UserCredentials.Include(c => c.Profile).FirstOrDefault(c => c.Id == user.Id);
        if (credential != null && credential.Profile != null)
        {
            credential.Profile.FullName = user.Name;
            _db.SaveChanges();
        }
    }
}

internal static class UserCredentialExtensions
{
    public static User? ToUser(this UserCredential? credential)
    {
        if (credential is null) return null;
        return new User
        {
            Id = credential.Id,
            Name = credential.Profile?.FullName ?? string.Empty,
            Email = credential.Email,
            PasswordHash = credential.PasswordHash,
            Role = credential.Role
        };
    }
}

public class InMemoryUserStore : IUserStore
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public User? GetByEmail(string email) =>
        _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    public User? GetById(int id) =>
        _users.FirstOrDefault(u => u.Id == id);

    public IEnumerable<User> GetAll() => _users.AsReadOnly();

    public User Add(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
        return user;
    }

    public bool EmailExists(string email) =>
        _users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

    public void UpdateUser(User user)
    {
        var existing = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existing != null)
        {
            existing.Name = user.Name;
        }
    }
}