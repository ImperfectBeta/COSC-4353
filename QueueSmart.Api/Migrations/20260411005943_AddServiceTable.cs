using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QueueSmart.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsOpen = table.Column<bool>(type: "boolean", nullable: false),
                    QueueLength = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "CreatedAt", "Description", "Duration", "IsOpen", "Name", "Priority", "QueueLength", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Walk-in general medical consultation.", 15, true, "General Consultation", 2, 8, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Submission and processing of official documents.", 10, true, "Document Processing", 1, 3, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), "IT and device troubleshooting desk.", 20, false, "Technical Support", 0, 0, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
