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

    public HistoryController(IHistoryStore historyStore, IServiceStore serviceStore)
    {
        _historyStore = historyStore;
        _serviceStore = serviceStore;
    }

    [HttpGet] // GET /api/history
    public ActionResult<IEnumerable<ServiceHistoryEntry>> GetAll([FromQuery] Guid? serviceId)
    {
        var entries = serviceId.HasValue
            ? _historyStore.GetByServiceId(serviceId.Value)
            : _historyStore.GetAll();

        return Ok(entries);
    }

    [HttpGet("statistics")] // GET /api/history/statistics
    public ActionResult<ServiceStatisticsResponse> GetStatistics()
    {
        var stats = _historyStore.GetStatistics(_serviceStore);
        return Ok(stats);
    }
}
