using Microsoft.AspNetCore.Mvc;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;
using QueueSmart.Api.Services;

namespace QueueSmart.Api.Controllers;

// controller for services

[ApiController]
[Route("api/[controller]")] // base route is /api/services
public class ServicesController : ControllerBase
{
    private readonly IServiceStore _serviceStore;
    private readonly IHistoryStore _historyStore;
    private readonly IQueueStore _queueStore;
    private readonly IQueueService _queueService;

    public ServicesController(IServiceStore serviceStore, IHistoryStore historyStore, IQueueStore queueStore, IQueueService queueService)
    {
        _serviceStore = serviceStore;
        _historyStore = historyStore;
        _queueStore = queueStore;
        _queueService = queueService;
    }

    [HttpGet] // get /api/services
    public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetAll()
    {
        var services = await _serviceStore.GetAllAsync();
        var activeQueues = await _queueStore.GetActiveQueuesAsync();
        
        var response = services.Select(s => 
        {
            var activeQ = activeQueues.FirstOrDefault(q => q.ServiceId == s.Id);
            int length = activeQ != null ? _queueService.GetQueue(activeQ.Id).Count : 0;
            return ServiceResponse.FromService(s, activeQ, length);
        });
        return Ok(response);
    }

    [HttpGet("{id:guid}")] // get /api/services/{id}
    public async Task<ActionResult<ServiceResponse>> GetById(Guid id)
    {
        var service = await _serviceStore.GetByIdAsync(id);
        if (service == null)
            return NotFound();

        var activeQueue = await _queueStore.GetActiveQueueForServiceAsync(id);
        int length = activeQueue != null ? _queueService.GetQueue(activeQueue.Id).Count : 0;
        return Ok(ServiceResponse.FromService(service, activeQueue, length));
    }

    [HttpPost] // post /api/services
    public async Task<ActionResult<ServiceResponse>> Create([FromBody] CreateServiceRequest request)
    {
        var service = new Service
        {
            Name = request.Name,
            Description = request.Description,
            Duration = request.Duration,
            Priority = request.Priority
        };

        var created = await _serviceStore.AddAsync(service);

        _historyStore.Record(new ServiceHistoryEntry
        {
            ServiceId = created.Id,
            ServiceName = created.Name,
            Action = "Created",
            Details = $"Service '{created.Name}' created with {created.Priority} priority and {created.Duration} min duration."
        });

        var response = ServiceResponse.FromService(created, null, 0);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    [HttpPut("{id:guid}")] // put /api/services/{id}
    public async Task<ActionResult<ServiceResponse>> Update(Guid id, [FromBody] UpdateServiceRequest request)
    {
        var updated = new Service
        {
            Name = request.Name,
            Description = request.Description,
            Duration = request.Duration,
            Priority = request.Priority
        };

        var result = await _serviceStore.UpdateAsync(id, updated);
        if (result == null)
            return NotFound();

        _historyStore.Record(new ServiceHistoryEntry
        {
            ServiceId = id,
            ServiceName = result.Name,
            Action = "Updated",
            Details = $"Service '{result.Name}' updated."
        });

        var activeQueue = await _queueStore.GetActiveQueueForServiceAsync(id);
        int length = activeQueue != null ? _queueService.GetQueue(activeQueue.Id).Count : 0;
        return Ok(ServiceResponse.FromService(result, activeQueue, length));
    }

    [HttpDelete("{id:guid}")] // delete /api/services/{id}
    public async Task<IActionResult> Delete(Guid id)
    {
        var service = await _serviceStore.GetByIdAsync(id);
        if (service == null)
            return NotFound();

        // optionally close queues if we're deleting the service
        var activeQueue = await _queueStore.GetActiveQueueForServiceAsync(id);
        if (activeQueue != null)
            await _queueStore.CloseQueueAsync(activeQueue.Id);

        await _serviceStore.DeleteAsync(id);

        _historyStore.Record(new ServiceHistoryEntry
        {
            ServiceId = id,
            ServiceName = service.Name,
            Action = "Deleted",
            Details = $"Service '{service.Name}' deleted."
        });

        return NoContent();
    }
}
