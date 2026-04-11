using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services
{
    public interface IQueueService
    {
        QueueEntryResponse JoinQueue(JoinQueueRequest request);
        bool LeaveQueue(int entryId, int userId);
        List<QueueEntryResponse> GetQueue(Guid queueId);
        QueueEntryResponse? ServeNext(Guid queueId);
    }
}