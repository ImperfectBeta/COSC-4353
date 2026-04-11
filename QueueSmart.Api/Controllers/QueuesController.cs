using Microsoft.AspNetCore.Mvc;
using QueueSmart.Api.Models;
using QueueSmart.Api.Services;

namespace QueueSmart.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // api/queues
public class QueuesController : ControllerBase
{
    private readonly IQueueStore _queueStore;

    public QueuesController(IQueueStore queueStore)
    {
        _queueStore = queueStore;
    }

    [HttpPost("open/{serviceId:guid}")]
    public async Task<ActionResult<Queue>> OpenQueue(Guid serviceId)
    {
        var queue = await _queueStore.OpenQueueAsync(serviceId);
        return Ok(queue);
    }

    [HttpPut("close/{serviceId:guid}")]
    public async Task<ActionResult<Queue>> CloseQueue(Guid serviceId)
    {
        var activeQueue = await _queueStore.GetActiveQueueForServiceAsync(serviceId);
        if (activeQueue == null)
            return NotFound("No active queue found for this service.");

        var queue = await _queueStore.CloseQueueAsync(activeQueue.Id);
        return Ok(queue);
    }
}
