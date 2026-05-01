using Microsoft.AspNetCore.Mvc;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Models;
using QueueSmart.Api.Services;

namespace QueueSmart.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // base route is /api/history
public class HistoryController : ControllerBase
{
    private readonly IHistoryStore _historyStore;
    private readonly IServiceStore _serviceStore;
    private readonly IQueueStore _queueStore;

    public HistoryController(IHistoryStore historyStore, IServiceStore serviceStore, IQueueStore queueStore)
    {
        _historyStore = historyStore;
        _serviceStore = serviceStore;
        _queueStore = queueStore;
    }

    [HttpGet] 
    public async Task<ActionResult<IEnumerable<ServiceHistoryEntry>>> GetAll([FromQuery] Guid? serviceId)
    {
        var entries = serviceId.HasValue
            ? await _historyStore.GetByServiceIdAsync(serviceId.Value)
            : await _historyStore.GetAllAsync();

        return Ok(entries);
    }

    [HttpGet("statistics")] 
    public async Task<ActionResult<ServiceStatisticsResponse>> GetStatistics()
    {
        var adminId = GetCurrentAdminId();
        if (adminId == null) return Unauthorized("Missing User ID header.");

        var stats = await _historyStore.GetStatisticsAsync(_serviceStore, _queueStore, adminId.Value);
        return Ok(stats);
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