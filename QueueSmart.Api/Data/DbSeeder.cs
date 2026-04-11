using QueueSmart.Api.Models;

namespace QueueSmart.Api.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Services.Any())
            {
                var services = new List<Service>
                {
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "General Support",
                        Description = "Get help with general account issues and questions.",
                        Duration = 10,
                        Priority = PriorityLevel.Medium,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Emergency Room",
                        Description = "Urgent care for critical medical conditions.",
                        Duration = 45,
                        Priority = PriorityLevel.High,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "DMV Renewal",
                        Description = "License and registration renewals.",
                        Duration = 120,
                        Priority = PriorityLevel.Low,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Id = Guid.NewGuid(),
                        Name = "Academic Advising",
                        Description = "Spring semester course selection advice.",
                        Duration = 15,
                        Priority = PriorityLevel.Medium,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                context.Services.AddRange(services);

                // Open active queues for all these services automatically!
                var queues = services.Select(s => new Queue
                {
                    Id = Guid.NewGuid(),
                    ServiceId = s.Id,
                    Status = "open",
                    QueueLength = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }).ToList();

                context.Queues.AddRange(queues);

                context.SaveChanges();
            }
        }
    }
}
