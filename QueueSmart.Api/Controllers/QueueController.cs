using System;
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
            if (request.UserId <= 0) return BadRequest("UserId is required.");
            if (request.QueueId == Guid.Empty) return BadRequest("QueueId is required.");

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
            if (userId <= 0) return BadRequest("UserId is required.");
            bool success = _queueService.LeaveQueue(entryId, userId);
            return success ? Ok("Removed from queue.") : NotFound("Queue entry not found.");
        }

        [HttpGet("{queueId}")]
        public IActionResult GetQueue(Guid queueId)
        {
            if (queueId == Guid.Empty) return BadRequest("QueueId is required.");
            var entries = _queueService.GetQueue(queueId);
            return Ok(new { entries = entries });
        }

        [HttpGet("user/{userId:int}")]
        public IActionResult GetUserEntries(int userId)
        {
            if (userId <= 0) return BadRequest("UserId is required.");
            var entries = _queueService.GetUserEntries(userId);
            return Ok(entries);
        }

        [HttpPost("serve-next/{queueId}")]
        public IActionResult ServeNext(Guid queueId)
        {
            if (queueId == Guid.Empty) return BadRequest("QueueId is required.");
            var next = _queueService.ServeNext(queueId);
            return next != null ? Ok(next) : NotFound("No users in queue.");
        }

        // Endpoint to accept the reorder request
        [HttpPut("{queueId}/reorder")]
        public IActionResult ReorderQueue(Guid queueId, [FromBody] ReorderQueueRequest request)
        {
            if (queueId == Guid.Empty) return BadRequest("QueueId is required.");
            if (request.OrderedEntryIds == null || request.OrderedEntryIds.Count == 0) 
                return BadRequest("No entries provided.");

            _queueService.ReorderQueue(queueId, request.OrderedEntryIds);
            return Ok("Queue reordered.");
        }
    }
}