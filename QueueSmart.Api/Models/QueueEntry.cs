namespace QueueSmart.Api.Models
{
    public class QueueEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public DateTime JoinedAt { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; } = "waiting";
    }
}