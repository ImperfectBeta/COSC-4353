using System.Data;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
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
        public IActionResult JoinQueue([FromBody] JoinQueueRequest request)
        {
            if (request.UserId <= 0)
                return BadRequest("UserId is required.");
            if (request.ServiceId <= 0)
                return BadRequest("ServiceId is required.");
            if (request.Priority < 1 || request.Priority > 3)
                return BadRequest("Priority must be between 1 and 3.");

            try
            {
                var result = _queueService.JoinQueue(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("leave/{entryId}")]
        public IActionResult LeaveQueue(int entryId, [FromQuery] int userId)
        {
            if (userId <= 0)
                return BadRequest("UserId is required.");

            bool success = _queueService.LeaveQueue(entryId, userId);
            return success ? Ok("Removed from queue.") : NotFound("Queue entry not found.");
        }

        [HttpGet("{serviceId}")]
        public IActionResult GetQueue(int serviceId)
        {
            if (serviceId <= 0)
                return BadRequest("ServiceId is required.");

            var queue = _queueService.GetQueue(serviceId);
            return Ok(queue);
        }

        [HttpPost("serve-next/{serviceId}")]
        public IActionResult ServeNext(int serviceId)
        {
            if (serviceId <= 0)
                return BadRequest("ServiceId is required.");

            var next = _queueService.ServeNext(serviceId);
            return next != null ? Ok(next) : NotFound("No users in queue.");
        }
    }
}