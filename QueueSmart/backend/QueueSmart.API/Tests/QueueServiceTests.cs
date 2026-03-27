using QueueSmart.API.DTOs;
using QueueSmart.API.Services;
using Xunit;

namespace QueueSmart.Tests
{
    public class QueueServiceTests
    {
        private readonly QueueServiceTests _service = new();

        [Fact]
        public void JoinQueue_ShouldAddEntryWithWaitingStatus()
        {
            var request = new JoinQueueRequest { UserId = 1, ServiceId = 1, Priority = 1 };
            var result = _service.JoinQueue(request);
            Assert.Equal("waiting", result.Status);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public void JoinQueue_ShouldThrowIfAlreadyInQueue()
        {
            var request = new JoinQueueRequest { UserId = 2, ServiceId = 1, Priority = 1 };
            _service.JoinQueue(request);
            Assert.Throws<InvalidOperationException>(() => _service.JoinQueue(request));
        }

        [Fact]
        public void LeaveQueue_ShouldCancelEntry()
        {
            var request = new JoinQueueRequest { UserId = 3, ServiceId = 1, Priority = 1 };
            var entry = _service.JoinQueue(request);
            bool result = _service.LeaveQueue(entry.Id, 3);
            Assert.True(result);
        }

        [Fact]
        public void LeaveQueue_ShouldReturnFalseIfNotFound()
        {
            bool result = _service.LeaveQueue(999, 999);
            Assert.False(result);
        }

        [Fact]
        public void ServeNext_ShouldReturnHighestPriorityFirst()
        {
            _service.JoinQueue(new JoinQueueRequest { UserId = 4, ServiceId = 2, Priority = 1 });
            _service.JoinQueue(new JoinQueueRequest { UserId = 5, ServiceId = 2, Priority = 3 });

            var served = _service.ServeNext(2);
            Assert.Equal(5, served?.UserId);
        }

        [Fact]
        public void ServeNext_ShouldReturnNullIfQueueEmpty()
        {
            var result = _service.ServeNext(999);
            Assert.Null(result);
        }

        [Fact]
        public void GetQueue_ShouldReturnOnlyWaitingEntries()
        {
            _service.JoinQueue(new JoinQueueRequest { UserId = 6, ServiceId = 3, Priority = 1 });
            _service.ServeNext(3);
            _service.JoinQueue(new JoinQueueRequest { UserId = 7, ServiceId = 3, Priority = 1 });

            var queue = _service.GetQueue(3);
            Assert.All(queue, e => Assert.Equal("waiting", e.Status));
        }
    }
}