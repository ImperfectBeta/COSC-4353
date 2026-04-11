using Microsoft.AspNetCore.Mvc;
using QueueSmart.Api.DTOs;
using QueueSmart.Api.Services;

namespace QueueSmart.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly IQueueService _queueService;

        public QueueController(IQueueService queueService)
        {
            _queueService = queueService;
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinQueue([FromBody] JoinQueueRequest request)
        {
            if (request.UserId <= 0)
                return BadRequest("UserId is required.");
            if (request.ServiceId == Guid.Empty)
                return BadRequest("A valid ServiceId is required.");

            try
            {
                var result = await _queueService.JoinQueue(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("leave/{entryId}")]
        public async Task<IActionResult> LeaveQueue(int entryId, [FromQuery] int userId)
        {
            if (userId <= 0)
                return BadRequest("UserId is required.");

            bool success = await _queueService.LeaveQueue(entryId, userId);
            return success ? Ok("Removed from queue.") : NotFound("Queue entry not found.");
        }

        [HttpGet("{serviceId}")]
        public async Task<IActionResult> GetQueue(Guid serviceId)
        {
            if (serviceId == Guid.Empty)
                return BadRequest("A valid ServiceId is required.");

            var queue = await _queueService.GetQueue(serviceId);
            return Ok(queue);
        }

        [HttpPost("serve-next/{serviceId}")]
        public async Task<IActionResult> ServeNext(Guid serviceId)
        {
            if (serviceId == Guid.Empty)
                return BadRequest("A valid ServiceId is required.");

            var next = await _queueService.ServeNext(serviceId);
            return next != null ? Ok(next) : NotFound("No users in queue.");
        }
    }
}