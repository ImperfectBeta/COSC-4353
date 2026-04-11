using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.SignalR;

namespace QueueSmart.Api.Models;

public class QueueEntry
{
    [Key]
    public int QueueEntryId { get; set; }

    [Required]
    public Guid QueueId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int Position { get; set; }
    public DateTime JoinTime { get; set; } = DateTime.UtcNow;

    [Required]
    public string Status { get; set; } = "waiting";

    [ForeignKey("QueueId")]
    public Queue? Queue { get; set; }

    [ForeignKey("UserId")]
    public UserCredential? User { get; set; }
}