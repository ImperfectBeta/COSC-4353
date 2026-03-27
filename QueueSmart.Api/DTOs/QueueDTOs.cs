namespace QueueSmart.Api.DTOs
{
    public class JoinQueueRequest
    {
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int Priority { get; set; } = 1;
    }

    public class QueueEntryResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public DateTime JoinedAt { get; set; }
        public int Priority { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Position { get; set; }
        public int EstimatedWaitMinutes { get; set; }
    }
}