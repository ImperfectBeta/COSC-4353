using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QueueSmart.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Queues",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Queues",
                columns: new[] { "Id", "CreatedAt", "QueueLength", "ServiceId", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), 8, new Guid("11111111-1111-1111-1111-111111111111"), "open", new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), 3, new Guid("22222222-2222-2222-2222-222222222222"), "open", new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "AdminId", "CreatedAt", "Description", "Duration", "Name", "Priority", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 0, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Walk-in general medical consultation.", 15, "General Consultation", 2, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 0, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Submission and processing of official documents.", 10, "Document Processing", 1, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), 0, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), "IT and device troubleshooting desk.", 20, "Technical Support", 0, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }
    }
}
