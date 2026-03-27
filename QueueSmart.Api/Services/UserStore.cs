using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services;

public interface IUserStore
{
    User? GetByEmail(string email);
    User? GetById(int id);
    IEnumerable<User> GetAll();
    User Add(User user);
    bool EmailExists(string email);
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
}