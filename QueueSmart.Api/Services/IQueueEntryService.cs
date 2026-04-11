using QueueSmart.Api.Models;

namespace QueueSmart.Api.Services;

public interface IQueueEntryService
{
    QueueEntry JoinQueue(Guid queueId, int userId);
    bool CancelEntry(int queueEntryId, int userId);
    List<QueueEntry> GetQueueEntries(Guid queueId);
    List<QueueEntry> GetHistoryForUser(int userId);
}