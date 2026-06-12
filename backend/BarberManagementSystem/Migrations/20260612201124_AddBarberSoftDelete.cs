using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddBarberSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Barbers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Barbers");
        }
    }
}
