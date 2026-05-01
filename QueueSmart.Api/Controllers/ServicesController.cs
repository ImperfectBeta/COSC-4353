using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    private readonly AppDbContext _context;

    public ServicesController(IServiceStore serviceStore, IHistoryStore historyStore, IQueueStore queueStore, AppDbContext context)
    {
        _serviceStore = serviceStore;
        _historyStore = historyStore;
        _queueStore = queueStore;
        _context = context;
    }

    [HttpGet] // get /api/services
    public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetAll()
    {
        var services = await _serviceStore.GetAllAsync();
        var activeQueues = await _queueStore.GetActiveQueuesAsync();
        
        var response = services.Select(s => 
        {
            var activeQ = activeQueues.FirstOrDefault(q => q.ServiceId == s.Id);
            int length = activeQ != null 
                ? _context.QueueEntries.Count(qe => qe.QueueId == activeQ.Id && qe.Status == "waiting") 
                : 0;
            return ServiceResponse.FromService(s, activeQ, length);
        });
        return Ok(response);
    }

    [HttpGet("admin")]
    public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetAdminServices()
    {
        var adminId = GetCurrentAdminId();
        if (adminId == null) return Unauthorized("User ID not found in headers.");

        var services = await _serviceStore.GetByAdminIdAsync(adminId.Value);
        var activeQueues = await _queueStore.GetActiveQueuesAsync();
        
        var response = services.Select(s => 
        {
            var activeQ = activeQueues.FirstOrDefault(q => q.ServiceId == s.Id);
            int length = activeQ != null 
                ? _context.QueueEntries.Count(qe => qe.QueueId == activeQ.Id && qe.Status == "waiting") 
                : 0;
            return ServiceResponse.FromService(s, activeQ, length);
        });
        return Ok(response);
    }

    [HttpGet("{id:guid}")] 
    public async Task<ActionResult<ServiceResponse>> GetById(Guid id)
    {
        var service = await _serviceStore.GetByIdAsync(id);
        if (service == null) return NotFound();

        var activeQueue = await _queueStore.GetActiveQueueForServiceAsync(id);
        int length = activeQueue != null 
            ? await _context.QueueEntries.CountAsync(qe => qe.QueueId == activeQueue.Id && qe.Status == "waiting") 
            : 0;
            
        return Ok(ServiceResponse.FromService(service, activeQueue, length));
    }

    [HttpPost] // post /api/services
    public async Task<ActionResult<ServiceResponse>> Create([FromBody] CreateServiceRequest request)
    {
        var adminId = GetCurrentAdminId();
        if (adminId == null) return Unauthorized("You must be logged in to create a service.");

        var service = new Service
        {
            Name = request.Name,
            Description = request.Description,
            Duration = request.Duration,
            Priority = request.Priority,
            AdminId = adminId.Value 
        };

        var created = await _serviceStore.AddAsync(service);
        var activeQueue = await _queueStore.OpenQueueAsync(created.Id);

        await _historyStore.RecordAsync(new ServiceHistoryEntry
        {
            ServiceId = created.Id,
            ServiceName = created.Name,
            Action = "Created",
            Details = $"Service '{created.Name}' created and queue opened."
        });

        var response = ServiceResponse.FromService(created, activeQueue, 0);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    [HttpPut("{id:guid}")] // put /api/services/{id}
    public async Task<ActionResult<ServiceResponse>> Update(Guid id, [FromBody] UpdateServiceRequest request)
    {
        var adminId = GetCurrentAdminId();
        if (adminId == null) return Unauthorized();

        var existing = await _serviceStore.GetByIdAsync(id);
        if (existing == null) return NotFound();
        if (existing.AdminId != adminId.Value) return Forbid("Permission denied.");

        var updated = new Service
        {
            Name = request.Name,
            Description = request.Description,
            Duration = request.Duration,
            Priority = request.Priority,
            AdminId = adminId.Value
        };

        var result = await _serviceStore.UpdateAsync(id, updated);
        if (result == null) return NotFound();

        await _historyStore.RecordAsync(new ServiceHistoryEntry
        {
            ServiceId = id,
            ServiceName = result.Name,
            Action = "Updated",
            Details = $"Service '{result.Name}' updated."
        });

        var activeQueue = await _queueStore.GetActiveQueueForServiceAsync(id);
        int length = activeQueue != null 
            ? await _context.QueueEntries.CountAsync(qe => qe.QueueId == activeQueue.Id && qe.Status == "waiting") 
            : 0;
            
        return Ok(ServiceResponse.FromService(result, activeQueue, length));
    }

    [HttpDelete("{id:guid}")] 
    public async Task<IActionResult> Delete(Guid id)
    {
        var adminId = GetCurrentAdminId();
        if (adminId == null) return Unauthorized();

        var service = await _serviceStore.GetByIdAsync(id);
        if (service == null) return NotFound();
        if (service.AdminId != adminId.Value) return Forbid("Permission denied.");

        var activeQueue = await _queueStore.GetActiveQueueForServiceAsync(id);
        if (activeQueue != null) await _queueStore.CloseQueueAsync(activeQueue.Id);

        await _serviceStore.DeleteAsync(id);

        await _historyStore.RecordAsync(new ServiceHistoryEntry
        {
            ServiceId = id,
            ServiceName = service.Name,
            Action = "Deleted",
            Details = $"Service '{service.Name}' deleted."
        });

        return NoContent();
    }

    private int? GetCurrentAdminId()
    {
        if (Request.Headers.TryGetValue("X-User-Id", out var userIdStr) && int.TryParse(userIdStr, out int adminId))
        {
            return adminId;
        }
        return null;
    }
}