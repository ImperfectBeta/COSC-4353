using QueueSmart.API.DTOs;
using QueueSmart.API.Models;

namespace QueueSmart.API.Services
{
    public interface IQueueService
    {
        QueueEntryResponse JoinQueue(JoinQueueRequest request);
        bool LeaveQueue(int entryId, int userId);
        List<QueueEntryResponse> GetQueue(int serviceId);
        QueueEntryResponse? ServeNext(int serviceId);
    }
}