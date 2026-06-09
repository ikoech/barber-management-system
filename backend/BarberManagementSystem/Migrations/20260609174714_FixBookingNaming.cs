using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class FixBookingNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Bookings",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Bookings",
                newName: "End");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Bookings",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Bookings",
                newName: "EndTime");
        }
    }
}
