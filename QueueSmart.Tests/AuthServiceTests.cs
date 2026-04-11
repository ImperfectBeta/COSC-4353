using QueueSmart.Api.DTOs;
using QueueSmart.Api.Services;

namespace QueueSmart.Tests;

public class AuthServiceTests
{
    private static AuthService CreateService() =>
        new AuthService(new InMemoryUserStore());

    // --- Register ---

    [Fact]
    public void Register_WithValidData_ReturnsAuthResponse()
    {
        var svc = CreateService();

        var (response, error) = svc.Register(new RegisterRequest
        {
            Name = "Alice Smith",
            Email = "alice@example.com",
            Password = "Password1"
        });

        Assert.Null(error);
        Assert.NotNull(response);
        Assert.Equal("alice@example.com", response.Email);
        Assert.Equal("Alice Smith", response.Name);
        Assert.False(string.IsNullOrEmpty(response.Token));
    }

    [Fact]
    public void Register_WithDuplicateEmail_ReturnsError()
    {
        var svc = CreateService();
        svc.Register(new RegisterRequest { Name = "Alice", Email = "alice@example.com", Password = "Password1" });

        var (response, error) = svc.Register(new RegisterRequest
        {
            Name = "Alice 2",
            Email = "alice@example.com",
            Password = "Password1"
        });

        Assert.Null(response);
        Assert.NotNull(error);
        Assert.Contains("already exists", error);
    }

    [Fact]
    public void Register_WithMissingName_ReturnsError()
    {
        var svc = CreateService();

        var (response, error) = svc.Register(new RegisterRequest
        {
            Name = "",
            Email = "alice@example.com",
            Password = "Password1"
        });

        Assert.Null(response);
        Assert.NotNull(error);
        Assert.Contains("Name is required", error);
    }

    [Fact]
    public void Register_WithShortPassword_ReturnsError()
    {
        var svc = CreateService();

        var (response, error) = svc.Register(new RegisterRequest
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "short"
        });

        Assert.Null(response);
        Assert.NotNull(error);
        Assert.Contains("8 characters", error);
    }

    [Fact]
    public void Register_WithInvalidEmail_ReturnsError()
    {
        var svc = CreateService();

        var (response, error) = svc.Register(new RegisterRequest
        {
            Name = "Alice",
            Email = "not-an-email",
            Password = "Password1"
        });

        Assert.Null(response);
        Assert.NotNull(error);
        Assert.Contains("invalid", error, StringComparison.OrdinalIgnoreCase);
    }

    // --- Login ---

    [Fact]
    public void Login_WithValidCredentials_ReturnsAuthResponse()
    {
        var svc = CreateService();
        svc.Register(new RegisterRequest { Name = "Bob", Email = "bob@example.com", Password = "Password1" });

        var (response, error) = svc.Login(new LoginRequest
        {
            Email = "bob@example.com",
            Password = "Password1"
        });

        Assert.Null(error);
        Assert.NotNull(response);
        Assert.Equal("bob@example.com", response.Email);
    }

    [Fact]
    public void Login_WithWrongPassword_ReturnsError()
    {
        var svc = CreateService();
        svc.Register(new RegisterRequest { Name = "Bob", Email = "bob@example.com", Password = "Password1" });

        var (response, error) = svc.Login(new LoginRequest
        {
            Email = "bob@example.com",
            Password = "WrongPassword"
        });

        Assert.Null(response);
        Assert.NotNull(error);
        Assert.Contains("Invalid", error);
    }

    [Fact]
    public void Login_WithUnknownEmail_ReturnsError()
    {
        var svc = CreateService();

        var (response, error) = svc.Login(new LoginRequest
        {
            Email = "nobody@example.com",
            Password = "Password1"
        });

        Assert.Null(response);
        Assert.NotNull(error);
        Assert.Contains("Invalid", error);
    }
}
