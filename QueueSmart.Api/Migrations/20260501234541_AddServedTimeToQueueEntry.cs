using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueSmart.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddServedTimeToQueueEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ServedTime",
                table: "QueueEntries",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServedTime",
                table: "QueueEntries");
        }
    }
}
