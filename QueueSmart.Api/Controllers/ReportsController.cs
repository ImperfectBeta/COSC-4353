using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QueueSmart.Api.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QueueSmart.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate, 
            [FromQuery] List<Guid> serviceIds,
            [FromQuery] string? status,
            [FromQuery] int? minWait,
            [FromQuery] int? maxWait)
        {
            var data = await GetFilteredReportDataAsync(startDate, endDate, serviceIds, status, minWait, maxWait);

            var totalJoined = data.Count;
            var totalServed = data.Count(x => x.OriginalStatus == "serving" || x.OriginalStatus == "completed");
            var totalCancelled = data.Count(x => x.OriginalStatus == "cancelled");

            var mostActiveService = data.GroupBy(x => x.ServiceName)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "None";

            var validWaitTimes = data.Where(x => x.WaitTimeMinutes.HasValue).ToList();
            var avgWaitTime = validWaitTimes.Any() ? validWaitTimes.Average(x => x.WaitTimeMinutes!.Value) : 0.0;

            return Ok(new
            {
                TotalUsersJoined = totalJoined,
                TotalUsersServed = totalServed,
                TotalCancelled = totalCancelled,
                MostActiveService = mostActiveService,
                AverageWaitTimeMinutes = Math.Round(avgWaitTime, 1)
            });
        }

        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportPdf(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate, 
            [FromQuery] List<Guid> serviceIds,
            [FromQuery] string? status,
            [FromQuery] int? minWait,
            [FromQuery] int? maxWait)
        {
            var data = await GetFilteredReportDataAsync(startDate, endDate, serviceIds, status, minWait, maxWait);

            var totalJoined = data.Count;
            var totalServed = data.Count(x => x.OriginalStatus == "serving" || x.OriginalStatus == "completed");
            var validWaitTimes = data.Where(x => x.WaitTimeMinutes.HasValue).ToList();
            var avgWaitTime = validWaitTimes.Any() ? Math.Round(validWaitTimes.Average(x => x.WaitTimeMinutes!.Value), 1) : 0;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial).FontColor(Colors.Black)); 

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("Statistics Report").SemiBold().FontSize(24);
                                column.Item().Text($"Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm UTC}").FontSize(10);
                            });
                        });
                        
                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Black);
                        
                        col.Item().PaddingVertical(10).Row(row => 
                        {
                            row.RelativeItem().Column(c => { c.Item().Text("Total Times Joined"); c.Item().Text(totalJoined.ToString()).FontSize(16).SemiBold(); });
                            row.RelativeItem().Column(c => { c.Item().Text("Total Users Served"); c.Item().Text(totalServed.ToString()).FontSize(16).SemiBold(); });
                            row.RelativeItem().Column(c => { c.Item().Text("Avg Wait Time"); c.Item().Text($"{avgWaitTime} min").FontSize(16).SemiBold(); });
                            
                            string dateRange = (startDate.HasValue ? startDate.Value.ToString("MMM dd") : "All Time") + " - " + 
                                               (endDate.HasValue ? endDate.Value.ToString("MMM dd") : "Present");
                            row.RelativeItem().Column(c => { c.Item().Text("Date Range"); c.Item().Text(dateRange).FontSize(14).SemiBold(); });
                        });
                        
                        col.Item().LineHorizontal(1).LineColor(Colors.Black);
                    });

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();   // Name
                            columns.RelativeColumn();   // Email
                            columns.RelativeColumn();   // Service
                            columns.ConstantColumn(60); // Priority
                            columns.RelativeColumn();   // Join Time
                            columns.RelativeColumn();   // Wait Time
                            columns.ConstantColumn(80); // Status
                        });

                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).Text("Customer Name").SemiBold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).Text("Email Address").SemiBold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).Text("Service Details").SemiBold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).Text("Priority").SemiBold(); // NEW
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).Text("Join Time").SemiBold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).Text("Wait Time").SemiBold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Black).PaddingBottom(5).Text("Status").SemiBold();
                        });

                        foreach (var item in data)
                        {
                            string waitTimeStr = item.WaitTimeMinutes.HasValue ? $"{item.WaitTimeMinutes.Value} min" : "N/A";

                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(item.FullName ?? "N/A");
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(item.Email ?? "N/A");
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(item.ServiceName ?? "N/A");
                            
                            
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(item.Priority);
                            
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(item.JoinTime.ToString("yyyy-MM-dd HH:mm"));
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(waitTimeStr);
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(item.DisplayStatus).SemiBold();
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"QueueSmart_MasterReport_{DateTime.UtcNow:yyyyMMdd}.pdf");
        }

        private async Task<List<ReportDataDto>> GetFilteredReportDataAsync(
            DateTime? startDate, DateTime? endDate, List<Guid> serviceIds, string? status, int? minWait, int? maxWait)
        {
            if (serviceIds == null || !serviceIds.Any())
            {
                return new List<ReportDataDto>();
            }

            var query = from qe in _context.QueueEntries
                        join up in _context.UserProfiles on qe.UserId equals up.Id
                        join uc in _context.UserCredentials on qe.UserId equals uc.Id
                        join q in _context.Queues on qe.QueueId equals q.Id
                        join s in _context.Services on q.ServiceId equals s.Id
                        select new 
                        {
                            up.FullName,
                            uc.Email,
                            ServiceName = s.Name,
                            ServiceId = s.Id,
                            Priority = s.Priority,
                            qe.JoinTime,
                            qe.ServedTime,
                            qe.Status
                        };

            if (startDate.HasValue) 
            {
                var startUtc = DateTime.SpecifyKind(startDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(x => x.JoinTime >= startUtc);
            }
            if (endDate.HasValue) 
            {
                var endUtc = DateTime.SpecifyKind(endDate.Value.Date, DateTimeKind.Utc).AddDays(1).AddTicks(-1);
                query = query.Where(x => x.JoinTime <= endUtc);
            }

            query = query.Where(x => serviceIds.Contains(x.ServiceId));
            query = query.OrderByDescending(x => x.JoinTime);

            var rawData = await query.ToListAsync();

            var reportData = rawData.Select(x => 
            {
                double? waitMins = null;
                
                if (x.Status == "cancelled") 
                {
                    waitMins = null; 
                }
                else if (x.ServedTime.HasValue)
                {
                    waitMins = (x.ServedTime.Value - x.JoinTime).TotalMinutes;
                }
                else 
                {
                    waitMins = (DateTime.UtcNow - x.JoinTime).TotalMinutes;
                }

                string displayStatus = x.Status switch
                {
                    "waiting" => "Active",
                    "serving" => "Active",
                    "completed" => "Completed",
                    "cancelled" => "Cancelled",
                    _ => x.Status
                };

                return new ReportDataDto
                {
                    FullName = x.FullName,
                    Email = x.Email,
                    ServiceName = x.ServiceName,
                    Priority = x.Priority.ToString(),
                    JoinTime = x.JoinTime,
                    OriginalStatus = x.Status, 
                    DisplayStatus = displayStatus, 
                    WaitTimeMinutes = waitMins.HasValue ? Math.Round(waitMins.Value, 1) : null
                };
            }).ToList();

            if (!string.IsNullOrEmpty(status)) reportData = reportData.Where(x => x.DisplayStatus.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            if (minWait.HasValue) reportData = reportData.Where(x => x.WaitTimeMinutes >= minWait.Value).ToList();
            if (maxWait.HasValue) reportData = reportData.Where(x => x.WaitTimeMinutes <= maxWait.Value).ToList();

            return reportData;
        }

        private class ReportDataDto
        {
            public string? FullName { get; set; }
            public string? Email { get; set; }
            public string? ServiceName { get; set; }
            public string Priority { get; set; } = string.Empty;
            public DateTime JoinTime { get; set; }
            public string OriginalStatus { get; set; } = string.Empty;
            public string DisplayStatus { get; set; } = string.Empty;
            public double? WaitTimeMinutes { get; set; }
        }
    }
}