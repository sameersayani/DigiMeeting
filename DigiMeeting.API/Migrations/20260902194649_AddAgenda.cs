using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigiMeeting.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAgenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Agenda",
                table: "Rooms",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Bookings",
                newName: "Agenda");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Rooms",
                newName: "Agenda");

            migrationBuilder.RenameColumn(
                name: "Agenda",
                table: "Bookings",
                newName: "Name");
        }
    }
}
