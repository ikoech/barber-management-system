using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBreaksModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "End",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "Breaks");

            migrationBuilder.AddColumn<string>(
                name: "DayOfWeek",
                table: "Breaks",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "Breaks",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "Breaks",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Breaks");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Breaks");

            migrationBuilder.AddColumn<DateTime>(
                name: "End",
                table: "Breaks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Start",
                table: "Breaks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
