using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services
{
    public interface IQueueService
    {
        QueueEntryResponse JoinQueue(JoinQueueRequest request);
        bool LeaveQueue(int entryId, int userId);
        List<QueueEntryResponse> GetQueue(int serviceId);
        QueueEntryResponse? ServeNext(int serviceId);
    }
}