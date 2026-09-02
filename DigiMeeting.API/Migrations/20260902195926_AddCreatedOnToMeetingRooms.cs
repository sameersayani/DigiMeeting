using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigiMeeting.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedOnToMeetingRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Rooms",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2026, 9, 2, 19, 59, 25, 28, DateTimeKind.Utc).AddTicks(6536));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedOn",
                value: new DateTime(2026, 9, 2, 19, 59, 25, 28, DateTimeKind.Utc).AddTicks(8385));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2026, 9, 2, 19, 59, 25, 28, DateTimeKind.Utc).AddTicks(8389));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2026, 9, 2, 19, 59, 25, 28, DateTimeKind.Utc).AddTicks(8390));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2026, 9, 2, 19, 59, 25, 28, DateTimeKind.Utc).AddTicks(8391));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedOn",
                value: new DateTime(2026, 9, 2, 19, 59, 25, 28, DateTimeKind.Utc).AddTicks(8392));

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedOn",
                value: new DateTime(2026, 9, 2, 19, 59, 25, 28, DateTimeKind.Utc).AddTicks(8394));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Rooms");
        }
    }
}
