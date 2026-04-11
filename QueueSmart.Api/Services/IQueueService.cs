using QueueSmart.Api.DTOs;

namespace QueueSmart.Api.Services
{
    public interface IQueueService
    {
        Task<QueueEntryResponse> JoinQueue(JoinQueueRequest request);
        Task<bool> LeaveQueue(int entryId, int userId);
        Task<List<QueueEntryResponse>> GetQueue(Guid serviceId);
        Task<QueueEntryResponse?> ServeNext(Guid serviceId);
    }
}