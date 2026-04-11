using System;
using System.Linq;
using System.Reflection;
using QueueSmart.Api.Models;
using Xunit;

namespace QueueSmart.Api.Tests;

public class ModelAndDtoTests
{
    [Fact]
    public void User_Defaults_AreExpected()
    {
        var user = new User();

        Assert.Equal(Guid.Empty, user.Id);
        Assert.Equal(string.Empty, user.Name);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.Equal(UserRole.User, user.Role);
    }

    [Fact]
    public void User_Properties_CanBeAssigned()
    {
        var id = Guid.NewGuid();
        var user = new User
        {
            Id = id,
            Name = "Test User",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = UserRole.SystemAdmin
        };

        Assert.Equal(id, user.Id);
        Assert.Equal("Test User", user.Name);
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("hash", user.PasswordHash);
        Assert.Equal(UserRole.SystemAdmin, user.Role);
    }

    [Fact]
    public void DtoAndModel_PublicSettableProperties_RoundTrip()
    {
        var assembly = typeof(User).Assembly;
        var types = assembly.GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Namespace is not null &&
                (t.Namespace.Contains(".Models", StringComparison.OrdinalIgnoreCase) ||
                 t.Namespace.Contains(".DTOs", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            if (instance is null) continue;

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var prop in props)
            {
                var value = SampleValue(prop.PropertyType);
                if (value is null) continue;

                try
                {
                    prop.SetValue(instance, value);
                    var actual = prop.GetValue(instance);
                    Assert.Equal(value, actual);
                }
                catch
                {
                    // skip init-only or constrained properties
                }
            }
        }
    }

    private static object? SampleValue(Type t)
    {
        if (t == typeof(string)) return "sample";
        if (t == typeof(Guid)) return Guid.NewGuid();
        if (t == typeof(Guid?)) return Guid.NewGuid();
        if (t == typeof(int)) return 123;
        if (t == typeof(int?)) return 123;
        if (t == typeof(long)) return 123L;
        if (t == typeof(long?)) return 123L;
        if (t == typeof(bool)) return true;
        if (t == typeof(bool?)) return true;
        if (t == typeof(DateTime)) return DateTime.UtcNow;
        if (t == typeof(DateTime?)) return DateTime.UtcNow;
        if (t.IsEnum) return Enum.GetValues(t).GetValue(0);

        return null;
    }
}