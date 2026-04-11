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

    public ServicesController(IServiceStore serviceStore, IHistoryStore historyStore)
    {
        _serviceStore = serviceStore;
        _historyStore = historyStore;
    }

    [HttpGet] // get /api/services
    public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetAll()
    {
        var services = await _serviceStore.GetAllAsync();
        var response = services.Select(ServiceResponse.FromService);
        return Ok(response);
    }

    [HttpGet("{id:guid}")] // get /api/services/{id}
    public async Task<ActionResult<ServiceResponse>> GetById(Guid id)
    {
        var service = await _serviceStore.GetByIdAsync(id);
        if (service == null)
            return NotFound();

        return Ok(ServiceResponse.FromService(service));
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

        var response = ServiceResponse.FromService(created);
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

        return Ok(ServiceResponse.FromService(result));
    }

    [HttpDelete("{id:guid}")] // delete /api/services/{id}
    public async Task<IActionResult> Delete(Guid id)
    {
        var service = await _serviceStore.GetByIdAsync(id);
        if (service == null)
            return NotFound();

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
