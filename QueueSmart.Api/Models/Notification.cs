using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueueSmart.Api.Models;

public class Notification
{
    [Key]
    public int NotificationId { get; set; } 

    [Required]
    public int UserId { get; set; } 

    [Required]
    public string Message { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [Required]
    public string Status { get; set; } = "sent"; 

    [ForeignKey("UserId")]
    public User? User { get; set; }
}