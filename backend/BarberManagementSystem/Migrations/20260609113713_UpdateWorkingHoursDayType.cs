using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWorkingHoursDayType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "WorkingHours");

            migrationBuilder.AddColumn<string>(
                name: "DayOfWeek",
                table: "WorkingHours",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "WorkingHours");

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "WorkingHours",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
