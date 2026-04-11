namespace QueueSmart.Api.DTOs
{
    public class JoinQueueRequest
    {
        public int UserId { get; set; }
        public Guid QueueId { get; set; }
    }

    public class QueueEntryResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Guid QueueId { get; set; }
        public DateTime JoinTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Position { get; set; }
        public int EstimatedWaitMinutes { get; set; }
    }
}