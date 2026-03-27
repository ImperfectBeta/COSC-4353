namespace QueueSmart.Api.Models;

// USER ROLE ENUM

public enum UserRole
{
    User,
    ServiceAdmin,
    SystemAdmin
}

// USER MODEL (whoever is doing the auth stuff can change this!!!!)

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;

}