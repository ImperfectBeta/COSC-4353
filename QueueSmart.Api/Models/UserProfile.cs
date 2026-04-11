using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueSmart.Api.Models;

[Table("UserProfiles")]
public class UserProfile
{
    // Shared PK with UserCredential (1-to-1)
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ContactInfo { get; set; }

    [MaxLength(500)]
    public string? Preferences { get; set; }

    public UserCredential Credential { get; set; } = null!;
}
