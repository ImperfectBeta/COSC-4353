using Microsoft.AspNetCore.Mvc;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;
using QueueSmart.Api.Services;

namespace QueueSmart.Api.Controllers;

// CONTROLLER FOR SERVICES

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

    [HttpGet] // GET /api/services
    public ActionResult<IEnumerable<ServiceResponse>> GetAll()
    {
        var services = _serviceStore.GetAll()
            .Select(ServiceResponse.FromService);
        return Ok(services);
    }

    [HttpGet("{id:guid}")] // GET /api/services/{id}
    public ActionResult<ServiceResponse> GetById(Guid id)
    {
        var service = _serviceStore.GetById(id);
        if (service == null)
            return NotFound();

        return Ok(ServiceResponse.FromService(service));
    }

    [HttpPost] // POST /api/services
    public ActionResult<ServiceResponse> Create([FromBody] CreateServiceRequest request)
    {
        var service = new Service
        {
            Name = request.Name,
            Description = request.Description,
            Duration = request.Duration,
            Priority = request.Priority
        };

        var created = _serviceStore.Add(service);

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

    [HttpPut("{id:guid}")] // PUT /api/services/{id}
    public ActionResult<ServiceResponse> Update(Guid id, [FromBody] UpdateServiceRequest request)
    {
        var updated = new Service
        {
            Name = request.Name,
            Description = request.Description,
            Duration = request.Duration,
            Priority = request.Priority
        };

        var result = _serviceStore.Update(id, updated);
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

    [HttpDelete("{id:guid}")] // DELETE /api/services/{id}
    public IActionResult Delete(Guid id)
    {
        var service = _serviceStore.GetById(id);
        if (service == null)
            return NotFound();

        _serviceStore.Delete(id);

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
